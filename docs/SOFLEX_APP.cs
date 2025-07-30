using System;
using System.Globalization;
using System.Net.Sockets;

namespace Parseos
{
	class SOFLEX_APP : Parser
	{

		private static string Prefijo = "";
		private static UdpClient udpSocket = new UdpClient();

		public SOFLEX_APP(string prefijo = null)
		{
			Prefijo = prefijo ?? "";
			PaqueteEnString = true;
			udpSocket = new UdpClient();
		}

		public override bool? IsValid(object cadenaOriginal)
		{
			return cadenaOriginal.ToString().Trim().StartsWith("SOF;");
		}


		public override string getCodigoGPS(object cadena)
		{
			return Prefijo + cadena.ToString().Trim().Split(';')[1];
		}

		public override byte[] getAck(object cadena)
		{
			return null;
		}

		public override string getData(object cadenaOriginal)
		{
			string[] campos = cadenaOriginal.ToString().Trim().Split(';');
			string codigoGPS = getCodigoGPS(cadenaOriginal);

			if (campos[0] == "SOF")
			{
				/*
				  SOF;0354532656262;20210219150015;-34.48251;-58.4264;50;125;20;95;15
					SOF				Indicador inicio Trama
					0354532656262	IMEI
					20210219150012	Fecha y Hora UTC0
                    -34.48251		Latitud
					-58.4264		Longitud
					50				Velocidad
					125				Sentido
					20				Evento
					95				Bateria
					15				CID

                */
				DateTime fechaGPS = DateTime.ParseExact(campos[2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

				if (fechaGPS > DateTime.Now.AddHours(1))
					return cadenaOriginal.ToString();

				float latitud = float.Parse(campos[3], CultureInfo.InvariantCulture);
				float longitud = float.Parse(campos[4], CultureInfo.InvariantCulture);
				float velocidad = float.Parse(campos[5]);
				short sentido = Convert.ToInt16(campos[6]);
				int nroEvento = Convert.ToInt32(campos[7]);
				float bateria = float.Parse(campos[8]);
				int cid = Convert.ToInt32(campos[9]);

				Console.WriteLine("CodigoGPS: " + codigoGPS);

				XGatewayBase.XGateway.EnviarPosicion(
					   CodigoGPS: codigoGPS,
					   FechaGPS: fechaGPS,
					   Latitud: latitud,
					   Bateria: bateria,
					   CID: cid,
					   Longitud: longitud,
					   Velocidad: velocidad,
					   Heading: sentido,
					   Evento: nroEvento,
					   TramaOriginal: cadenaOriginal.ToString()
				);
			}

			return cadenaOriginal.ToString();
		}
	}
}
