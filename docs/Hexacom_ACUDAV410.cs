using System;
using System.Text;
using mzcoreSharp.CORE;
using System.Configuration;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Parseos
{
	class Hexacom_ACUDAV410 : Parser
	{

        private static string Prefijo = "";
		private static string CCCC = ""; //CCCC = Numero de abonado
		private static string ZZZ = ""; //ZZZ = Identificador de dispositivo
		private static UdpClient udpSocket;

		public Hexacom_ACUDAV410(string prefijo = null)
		{
			Prefijo = prefijo ?? "HC-";
			PaqueteEnString = true;
			udpSocket = new UdpClient();
		}

		public override object LimpiarPaquete(object cadenaOriginal)
		{
            ParseData = ""; 
			string cadena = cadenaOriginal.ToString().Trim().Replace(" ", "");
			return cadena;
		}

        public override string getParseData()
        {
            return ParseData;
        }


        public override bool? IsValid(object cadenaOriginal)
		{
			string cadena = cadenaOriginal.ToString();
			string PP = cadena.Substring(0, 2);
			if ((PP == "18") && (cadena.Length == 16)) //PP = Contact ID = 18  / cadena = PPCCCCQQEEEGGZZZ (16 caracteres)
				return true;
			else
				return false;
		}


		public override string getCodigoGPS(object cadenaOriginal)
		{
			string cadena = cadenaOriginal.ToString(); //cadena = PPCCCCQQEEEGGZZZ (16 caracteres)
			CCCC = cadena.Substring(2, 4); //CCCC = ID de Abonado (Numero de abonado)
			ZZZ = cadena.Substring(13, 3); //ZZZ = ID de Zona (Identificador de dispositivo)
			var result = CCCC + ZZZ;
			if ((result != null) && (Prefijo != null) && (Prefijo != "")) { 
				result = Prefijo + result;
			}
			return result;
		}

		public override byte[] getAck(object cadenaOriginal)
		{
			string cadena = cadenaOriginal.ToString();
			return Encoding.ASCII.GetBytes(cadena);
		}

		public override string getData(object cadenaOriginal)
		{
            /*
			Paquete: PP CCCC QQ EEE GG ZZZ
			Ejemplo: 18 4321 01 602 01 005
			
			PP		= Especifica el protocolo. Contact ID = 18
			CCCC	= Número de abonado
			QQ		= Calificador de evento 01 Evento Nuevo, 03 Restauración de evento.
			EEE		= Código de evento.
			GG		= Partición.En ACUDAV410 GG = 01
			ZZZ		= Identificador de Zona. Para el panel ACUDA las zonas 1,2 y 3 son cableadas y para valores de ZZZ > 3 identifica el
			dispositivo inalámbrico de las zonas RF. El número de zona RF se debe incluir en la descripción del campo ZZZ dentro del software
			de monitoreo.
			*/

            string cadena = cadenaOriginal.ToString();

			string codigoGPS = getCodigoGPS(cadena);
			DateTime fechaGPS = DateTime.Now;			
			float latitud = 0; /*Equipo que no reporta posicion GPS*/
			float longitud = 0; 
            float velocidad = 0;
			short sentido = 0;
			int entradasDigitales = 0;
			bool ign = false;
			float fuente = 0;
			int flags = 0;
			byte satelites = 0;

			string QQ = cadena.Substring(6, 2);
			string EEE = cadena.Substring(8, 3);
			string EventoStr = QQ + EEE;
			int nroEvento = Convert.ToInt32(EventoStr);			

			if ((latitud == 0) && (longitud == 0))
			{
				device_location dlocation;
				dlocation = getDeviceLocationFromJson(CCCC);
				if (dlocation != null)
				{
					if ((dlocation.lat != 0) && (dlocation.lon != 0))
					{
						latitud = dlocation.lat;
						longitud = dlocation.lon;
						Logs.Log("Ubicacion desde json: ID_Abonado=" + CCCC + " ID_Zona=" + ZZZ+" Evento="+ EventoStr+" lat=" + latitud.ToString().Replace(",", ".") + " lon=" + longitud.ToString().Replace(",", "."));
					}
				} else
                {
					Logs.Log("Sin ubicacion desde json: ID_Abonado=" + CCCC + " ID_Zona=" + ZZZ + " Evento=" + EventoStr + " -> dlocation == null");
				}
			}

			//Console.WriteLine("cadena: " + cadena);
			//Console.WriteLine("QQ: " + QQ);
			//Console.WriteLine("EEE: " + EEE);
			//Console.WriteLine("EventoStr: " + EventoStr);
			//Console.WriteLine("nroEvento: " + nroEvento.ToString());
			
			//Console.WriteLine("CodigoGPS: " + codigoGPS);
			//Console.WriteLine("ID_Abonado: " + CCCC + " ID_Zona: " + ZZZ);
			//Console.WriteLine("lat: " + latitud.ToString().Replace(",", "."));
			//Console.WriteLine("lon: " + longitud.ToString().Replace(",", "."));

			ParseData = "codigoGPS=" + codigoGPS + " " +
				"ID_Abonado: " + CCCC + " " +
				"ID_Zona: " + ZZZ + " " +
				"lat: " + latitud.ToString().Replace(",", ".") + " " +
				"lon: " + longitud.ToString().Replace(",", ".");

            if (ZZZ != "000")
            {
				XGatewayBase.XGateway.EnviarPosicion(
                       CodigoGPS: codigoGPS,
                       FechaGPS: fechaGPS,
                       Latitud: (float) latitud,
                       Longitud: (float) longitud,
                       Velocidad: velocidad,
                       Heading: sentido,
                       Evento: nroEvento,
                       EntradasDigitales: entradasDigitales,
                       Ignicion: ign,
                       Fuente: fuente,
                       TramaOriginal: cadenaOriginal.ToString(),
                       EstadoGPS: flags,
                       Satelites: satelites
				);
			
				if (ConfigurationManager.AppSettings["IPDestinoGenerico"] != null)
				{
					try
					{
						/*
						string datos = "|CodigoGPS=" + codigoGPS +
							",FechaGPS=" + fechaGPS.ToUniversalTime().ToString("yyyyMMddHHmmss") +
							",Latitud=" + latitud.ToString() +
							",Longitud=" + longitud.ToString() +
							",Velocidad=" + velocidad.ToString() +
							",Heading=" + sentido.ToString() +
							",Evento=" + nroEvento.ToString()
							+ "|";
						*/
						string datos = "<Soflex>" +
						//"providerid=0" + ";" +
						//"clientid=0" + ";" +
						//"deviceid=0" + ";" +
						"trackingid=" + codigoGPS + ";" +
						"date=" + fechaGPS.ToUniversalTime().ToString("yyyyMMddHHmmss") + ";" +
						"lat=" + latitud.ToString().Replace(",", ".") + ";" +
						"lon=" + longitud.ToString().Replace(",", ".") + ";" +
						//"alt=" + altitud.ToString() + ";" +
						"speed=" + velocidad.ToString().Replace(",", ".") + ";" +
						"bearing=" + sentido.ToString() + ";" +
						//acc = 4; bat = 62; type = 1; online = 1; is_alarm=false ;
						"event=" + nroEvento.ToString();

						var envio = Encoding.ASCII.GetBytes(datos);
						udpSocket.Send(envio, envio.Length, ConfigurationManager.AppSettings["IPDestinoGenerico"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["PuertoDestinoGenerico"]));
						Console.WriteLine("Reenviado: " + datos);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error al reenviar dato <Soflex>: " + ex.Message);
					}
				}

				if (ConfigurationManager.AppSettings["IPDestinoRGP"] != null)
				{
					try
					{
						string aux_velocidad = "000" + Convert.ToInt32((velocidad * 0.621371)).ToString();
						string datos = ">RPF"
						+ fechaGPS.ToUniversalTime().ToString("ddMMyyHHmmss") //FechaGPS
						+ (latitud.ToString().Replace(".", "") + "00000000").Substring(0, 8) //Latitud
						+ ("-0" + longitud.ToString().Replace("-", "").Replace(".", "") + "000000000").Substring(0, 9) //Longitud
						+ aux_velocidad.Substring(aux_velocidad.Length - 3) //Velocidad
						+ ("000" + sentido.ToString()).Substring(("000" + sentido.ToString()).Length - 3) //Sentido
						+ "0"   //Satelite
						+ "00"  //Edad
						+ "00"  //Estado entradas
						+ ("00" + nroEvento.ToString()).Substring(("00" + nroEvento.ToString()).Length - 2)  //Evento
						+ ";ID=" + codigoGPS + ";#0002;*52<";

						var envio = Encoding.ASCII.GetBytes(datos);
						udpSocket.Send(envio, envio.Length, ConfigurationManager.AppSettings["IPDestinoRGP"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["PuertoDestinoRGP"]));
						Console.WriteLine("Reenviando: " + datos);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error al reenviar dato RGP: " + ex.Message);
					}
				}

				Console.WriteLine("Envio al gateway: " + codigoGPS + " " + fechaGPS.ToLongDateString() + " " + latitud.ToString() + " " + longitud.ToString());
			} else
            {
				Console.WriteLine("Dato no enviado por poseer ID_Zona no identificada: 000");
			}

			return cadena;
		}
	}
}
