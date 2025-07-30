using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Configuration;
using System.Net.Sockets;

namespace Parseos
{
	public class Meitrack : Parser
	{
		private static string Prefijo = "";
		private static UdpClient udpSocket = new UdpClient();

		public Meitrack(string prefijo = null)
		{
			Prefijo = prefijo ?? "MT-";
			udpSocket = new UdpClient();
		}

		public override bool? IsValid(object cadena)
		{
			
			string cadenaOriginal = cadena.ToString();
			bool valido = cadenaOriginal.StartsWith("$$");

			string[] partes = cadenaOriginal.Split(',');
			string aux = string.Join("", partes[0].Skip(3).ToArray());
			string resto = "," + string.Join(",", partes.Skip(1).ToArray());
			valido = valido && Convert.ToInt32(aux) == resto.Length;
			/* //NO FUNCIONA CON ESTA COMPARACION
			 * string chksum = cadenaOriginal.Split('*')[1].Substring(0,2);
			 * valido = valido && chksum == getCheckSum(cadenaOriginal);
			*/
			return valido;
		}


		public override string getCodigoGPS(object cadenaOriginal)
		{
			string[] aux = cadenaOriginal.ToString().Split(',');
			if (aux.Length > 1)
                return Prefijo + aux[1];
			return "";
		}

		public override string getData(object cadenaOriginal)
		{
			string CodigoGPS = getCodigoGPS(cadenaOriginal);


			string[] aux = cadenaOriginal.ToString().Split(',');
			List<string> valores = new List<string>();
			valores.Add(aux[3]);//3: EVENTO
			valores.Add(aux[4]);//4: LATITUD
			valores.Add(aux[5]);//5: LONGITUD
			string fecha = DateTime.Now.Year.ToString().Substring(0, 2) + string.Join("", aux[6].Take(6).ToArray());
			string hora = "" + aux[6][6] + aux[6][7] + ":" + aux[6][8] + aux[6][9] + ":" + aux[6][10] + aux[6][11];
			valores.Add(fecha + " " + hora);//6: FECHAGPS
			valores.Add(aux[7] == "A" ? "1" : "0");//7: POSICION VALIDA
			valores.Add(aux[8]);//8: NUMERO SATELITES
			valores.Add(aux[9]);//9: SEÑAL GSM
			valores.Add(aux[10]);//10: VELOCIDAD
			valores.Add(aux[11]);//11: SENTIDO
			valores.Add(aux[14]);//14: ODOMETRO
			valores.Add(aux[17]);//17: OUTPUTS E INPUTS
			string[] ad = aux[18].Split('|');
			float bateria = 0;
			if (ad.Length > 3)
				bateria = int.Parse(ad[3], System.Globalization.NumberStyles.HexNumber) / (float)100;
			valores.Add(bateria.ToString().Replace(',', '.'));//18: AD
			valores.Add((aux[3] == "37" ? aux[19] : ""));//SI 3 (EVENTO) == 37 ENTONCES RFID EN POS 19
			string dato = string.Join(",", valores.ToArray());


			string RFID = null;

			if (aux[3] == "37")
			{
				RFID = aux[19];
			}


			DateTime fechaGPS = DateTime.ParseExact(aux[6], "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
			float latitud = float.Parse(aux[4]);
			float longitud = float.Parse(aux[5]);
			float velocidad = float.Parse(aux[10]);
			short sentido = Convert.ToInt16(aux[11]);
			int evento = Convert.ToInt32(aux[3]);

			Console.WriteLine("CodigoGPS: " + CodigoGPS);
			Console.WriteLine("FechaGPS: " + fechaGPS.ToString("yyyy-MM-dd HH:mm:ss"));
			Console.WriteLine("Evento: " + evento.ToString());
			Console.WriteLine("Latitud: " + latitud.ToString());
			Console.WriteLine("Longitud: " + longitud.ToString());

			XGatewayBase.XGateway.EnviarPosicion(
				CodigoGPS: CodigoGPS,
				FechaGPS: fechaGPS,
				Evento: evento,
				Latitud: latitud,
				Longitud: longitud,
				EdadPosicion: (byte)(aux[7] == "A" ? 1 : 0),
				Satelites: Convert.ToByte(aux[8]),
				SenialGSM: Convert.ToByte(aux[9]),
				Velocidad: velocidad,
				Heading: sentido,
				EntradasDigitales: Convert.ToInt16(aux[17], 16),
				HDOP: float.Parse(aux[12]),
				Odometro: float.Parse(aux[14]),
				TramaOriginal: cadenaOriginal.ToString(),
				Analog1: (ad[0] == null || ad[0] == "") ? (float?)null : float.Parse(Convert.ToInt32(ad[0], 16).ToString()),
				Analog2: (ad[1] == null || ad[1] == "") ? (float?)null : float.Parse(Convert.ToInt32(ad[1], 16).ToString()),
				Analog3: (ad[2] == null || ad[2] == "") ? (float?)null : float.Parse(Convert.ToInt32(ad[2], 16).ToString()),
				Fuente: (ad[3] == null || ad[3] == "") ? (float?)null : float.Parse(Convert.ToInt32(ad[3], 16).ToString()) / 1024 * 3 * 2,
				RFID: RFID
			);


			if (ConfigurationManager.AppSettings["IPDestinoGenerico"] != null)
			{
				try
				{
					/*
					 string datos = "|CodigoGPS=" + CodigoGPS +
						",FechaGPS=" + fechaGPS.ToUniversalTime().ToString("yyyyMMddHHmmss") +
						",Latitud=" + latitud.ToString() +
						",Longitud=" + longitud.ToString() +
						",Velocidad=" + velocidad.ToString() +
						",Heading=" + sentido.ToString() + 
						",Evento=" + evento.ToString()
						+ "|";
					*/

					string datos = "<Soflex>" +
					"providerid=0" + ";" +
					"clientid=0" + ";" +
					"deviceid=0" + ";" +
					"trackingid=" + CodigoGPS + ";" +
					"date=" + fechaGPS.ToUniversalTime().ToString("yyyyMMddHHmmss") + ";" +
					"lat=" + latitud.ToString() + ";" +
					"lon=" + longitud.ToString() + ";" +
					//"alt=" + altitud.ToString() + ";" +
					"speed=" + velocidad.ToString() + ";" +
					"bearing=" + sentido.ToString() + ";" +
					//acc = 4; bat = 62; type = 1; online = 1; is_alarm=false ;
					"event=" + evento.ToString();

					var envio = Encoding.ASCII.GetBytes(datos);
					udpSocket.Send(envio, envio.Length, ConfigurationManager.AppSettings["IPDestinoGenerico"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["PuertoDestinoGenerico"]));
					Console.WriteLine("Reenviando: " + datos);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error al reenviar dato RGP: " + ex.Message);
				}
			}

			return dato;
		}

		public override byte[] getComando(object cadena, object aux)
		{
			return Encoding.ASCII.GetBytes(cadena.ToString() + (Char)13 + (Char)10);
		}

		public override byte[] getAck(object cadena)
		{
			//string chkSum = getCheckSum(RESPUESTA);
			return null;
		}

		public override object getCheckSum(object cadena)
		{
			return null;
			/*
			cadena = cadena.Split('*')[0] + "*";
			uint hex = 0;
			foreach (char c in cadena)
			{
				int tmp = c;
				hex += Convert.ToUInt32(tmp.ToString());
			}
			return string.Join("", String.Format("{0:x2}", hex).Reverse().Take(2).Reverse().ToArray());*/
		}


	}
}
