//using mzcoreSharp.Protocolo;
using System;
using System.Collections.Generic;
//using System.Collections.Generic;
using System.Configuration;
using System.Deployment.Internal;

//using System.Data.SqlTypes;
using System.Linq;
using System.Net.Sockets;
//using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
//using System.Web.Services.Description;
//using System.Xml.Linq;

namespace Parseos
{
    class Teltonika_FMC130 : Parser
    {
        private static string Prefijo = "";

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static int Length;
        private static int PackedID;
        private static int PacketType;
        private static int AVLPacketID;
        private static int ImeiLength;
        private static string Imei;
        private static int CodecID;
        private static int AVLDataCount;
        private static int AVLDataCountFinal;

        private class IOElement
        {
            public int ID;
            public long Value; //8 bytes
            public IOElement(int id, long value)
            {
                ID = id;
                Value = value;
            }
        }
        
        private class AVLData
        {
            public DateTime Timestamp;
            public int Priority;
            public float Longitude;
            public float Latitude;
            public int Altitude;
            public int Angle;
            public int Satellites;
            public int Speed;
            public int EventID;
            public int ElementCount;
            public int b1ElementCount;
            public int b2ElementCount;
            public int b4ElementCount;
            public int b8ElementCount;
            public int bXElementCount;
            public List<IOElement> b1ElementList;
            public List<IOElement> b2ElementList;
            public List<IOElement> b4ElementList;
            public List<IOElement> b8ElementList;
            public List<IOElement> bXElementList;
            public AVLData()
            {
                ClearAVLData();
                b1ElementList = new List<IOElement>();
                b2ElementList = new List<IOElement>();
                b4ElementList = new List<IOElement>();
                b8ElementList = new List<IOElement>();
                bXElementList = new List<IOElement>();
            }

            public void ClearAVLData()
            {
                Timestamp = DateTime.Now;
                Priority = 0;
                Longitude = 0;
                Latitude = 0;
                Altitude = 0;
                Angle = 0;
                Satellites = 0;
                Speed = 0;
                EventID = 0;
                ElementCount = 0;
            }

            public void ClearElementLists()
            {
                b1ElementList.Clear();
                b2ElementList.Clear();
                b4ElementList.Clear();
                b8ElementList.Clear();
                bXElementList.Clear();
            }

            public void Dispose()
            {
                ClearElementLists();
            }
        }
        
        private static List<AVLData> AVLDataList;

        public void ClearData()
        {
            Length = 0;
            PackedID = 0;
            PacketType = 0;
            AVLPacketID = 0;
            ImeiLength = 0;
            Imei = "";
            CodecID = 0;
            AVLDataCount = 0;

            for (int i = 0; i < AVLDataList.Count; i++)
            {
                AVLDataList[i].ClearAVLData();
                AVLDataList[i].ClearElementLists();
            }

            AVLDataList.Clear();
        }

        public Teltonika_FMC130(string prefijo = null)
        {
            Prefijo = prefijo ?? ""; 
            PaqueteEnString = false;
            AVLDataList = new List<AVLData>();
        }

        public override string getParseData()
        {
            string r = "";

            r += "Length=" + Length.ToString() + " ";
            r += "PackedID=" + PackedID.ToString() + " ";
            r += "PacketType=" + PacketType.ToString() + " ";
            r += "AVLPacketID=" + AVLPacketID.ToString() + " ";
            r += "ImeiLength=" + ImeiLength.ToString() + " ";
            r += "Imei=" + Imei + " ";
            r += "CodecID=" + CodecID.ToString() + " ";
            r += "AVLDataCount=" + AVLDataCount.ToString() + "\r\n";

            for (int i = 0; i < AVLDataList.Count; i++)
            {
                r += "----- AVLData[" + i.ToString() + "] -----" + "\r\n";
                r += "Timestamp=" + AVLDataList[i].Timestamp.ToString() + " ";
                r += "Priority=" + AVLDataList[i].Priority.ToString() + " ";
                r += "Longitude=" + AVLDataList[i].Longitude.ToString() + " ";
                r += "Latitude=" + AVLDataList[i].Latitude.ToString() + " ";
                r += "Altitude=" + AVLDataList[i].Altitude.ToString() + " ";
                r += "Angle=" + AVLDataList[i].Angle.ToString() + " ";
                r += "Satellites=" + AVLDataList[i].Satellites.ToString() + " ";
                r += "Speed=" + AVLDataList[i].Speed.ToString() + " ";
                r += "EventID=" + AVLDataList[i].EventID.ToString() + " ";
                r += "ElementCount=" + AVLDataList[i].ElementCount.ToString() + "\r\n";

                r += "-- b1ElementCount=" + AVLDataList[i].b1ElementCount.ToString() + " --" + "\r\n";
                for (int j = 0; j < AVLDataList[i].b1ElementList.Count; j++)
                {
                    r += "ID=" + AVLDataList[i].b1ElementList[j].ID.ToString() + " ";
                    r += "Value=" + AVLDataList[i].b1ElementList[j].Value.ToString() + " ";
                    if (j == AVLDataList[i].b1ElementList.Count-1) {
                        r += "\r\n"; 
                    } else
                    {
                        r += "/ ";
                    }        
                }

                r += "-- b2ElementCount=" + AVLDataList[i].b2ElementCount.ToString() + " --" + "\r\n";
                for (int j = 0; j < AVLDataList[i].b2ElementList.Count; j++)
                {
                    r += "ID=" + AVLDataList[i].b2ElementList[j].ID.ToString() + " ";
                    r += "Value=" + AVLDataList[i].b2ElementList[j].Value.ToString() + " ";
                    if (j == AVLDataList[i].b2ElementList.Count - 1)
                    {
                        r += "\r\n";
                    }
                    else
                    {
                        r += "/ ";
                    }
                }

                r += "-- b4ElementCount=" + AVLDataList[i].b4ElementCount.ToString() + " --" + "\r\n";
                for (int j = 0; j < AVLDataList[i].b4ElementList.Count; j++)
                {
                    r += "ID=" + AVLDataList[i].b4ElementList[j].ID.ToString() + " ";
                    r += "Value=" + AVLDataList[i].b4ElementList[j].Value.ToString() + " ";
                    if (j == AVLDataList[i].b4ElementList.Count - 1)
                    {
                        r += "\r\n";
                    }
                    else
                    {
                        r += "/ ";
                    }
                }

                r += "-- b8ElementCount=" + AVLDataList[i].b8ElementCount.ToString() + " --" + "\r\n";
                for (int j = 0; j < AVLDataList[i].b8ElementList.Count; j++)
                {
                    r += "ID=" + AVLDataList[i].b8ElementList[j].ID.ToString() + " ";
                    r += "Value=" + AVLDataList[i].b8ElementList[j].Value.ToString() + " ";
                    if (j == AVLDataList[i].b8ElementList.Count - 1)
                    {
                        r += "\r\n";
                    }
                    else
                    {
                        r += "/ ";
                    }
                }

                r += "-- bXElementCount=" + AVLDataList[i].bXElementCount.ToString() + " --" + "\r\n";
                for (int j = 0; j < AVLDataList[i].bXElementList.Count; j++)
                {
                    r += "ID=" + AVLDataList[i].bXElementList[j].ID.ToString() + " ";
                    r += "Value=" + AVLDataList[i].bXElementList[j].Value.ToString() + " ";
                    if (j < AVLDataList[i].bXElementList.Count - 1)
                    {
                        r += "/ ";
                    }
                }

                if ((i < AVLDataList.Count - 1) && (AVLDataList.Count > 1))
                {
                    r += "\r\n";
                }

            }

            return r;
        }

        public override bool? IsValid(object cadenaOriginal)
        {
            return true;
        }

        public override byte[] getAck(object cadena)
        {
            /*
            ACK: 
                0005            4 bytes
                PackedID        4 bytes
                PacketType      2 bytes
                AVLPacketID     1 byte
                AVLDataCount    1 byte
            */

            int ImeiLength = BitConverter.ToInt16(getBytes((byte[])cadena, 6, 2, true), 0);
            int auxInicio = 8 + ImeiLength;

            byte[] array = { 0x00, 0x05 }; // 00 + 05
            array = array.Concat(getBytes((byte[])cadena, 2, 4, false)).ToArray(); //PackedID + PacketType + AVLPacketID
            array = array.Concat(getBytes((byte[])cadena, auxInicio + 1, 1)).ToArray(); //AVLDataCount
            return array;
        }

        public override string getCodigoGPS(object cadena)
        {
            string Imei_aux;
            if ((Imei == "") || (Imei == null))
            {
                int imeiLength = BitConverter.ToInt16(getBytes((byte[])cadena, 6, 2, true), 0);
                Imei_aux = Encoding.UTF8.GetString(getBytes((byte[])cadena, 8, imeiLength, false));
            } else
            {
                Imei_aux = Imei;
            }

            if ((Imei_aux != null) && (Imei_aux != "") && (Prefijo != null) && (Prefijo != ""))
            {
                Imei_aux = Prefijo + Imei_aux;
            }

            return Imei_aux;
        }


        public override string getData(object cadenaOriginal)
        {
            /* Ejemplo:
            00E6CAFE0117000F3836303839363035303232313633398E0100000196B4E8219000DD1EEAA0EB6264AF0019001713001900000021000A00EF0100F00100715B005114005200005900006F0000A00000E80000EB000009004237C20018001900430FBD00440000005400000055036C005A0000006E00000073032A00080010005F33550053000001850057010D2658006400002D4F006600000000006700000000006900000000006B0000000000050065007C00A519203030008400000000000000000205000000000000000002060000000000000000020700000000000000000001011A000001
            
            -- Header --
            Length          2 bytes
            Packet ID       2 bytes
            Packet Type     1 byte
            AVL paquet ID   1 byte
            Imei Length     2 bytes
            Imei            15 bytes
            -- Data --
            Codec ID        1 byte
            AVL Data Count  1 byte
                -- AVL Data -- (Contiene tantos "AVL Data" como el valor que trae en "AVL Data Count")
                Timestamp       8 bytes
                Priority        1 byte
                    -- GPS Element --
                    Longitude   4 bytes
                    Latitude    4 bytes
                    Altitude    2 bytes
                    Angle       2 bytes
                    Satellites  1 byte
                    Speed       2 bytes
                    -- I/O Element --
                    Event ID            2 bytes
                    Element count       2 bytes (Cantidad total de elementos)
                    1b Element count    2 bytes (Cantidad de elementos de 1 byte) (Hay tantos elementos de 1 byte como el valor que posee "1b Element count")
                    ...
                    ID                  2 bytes
                    Value               1 byte
                    ...
                    2b Element count    2 bytes (Cantidad de elementos de 2 bytes) (Hay tantos elementos de 2 bytes como el valor que posee "2b Element count")
                    ...
                    ID                  2 bytes
                    Value               2 bytes
                    ...
                    4b Element count    2 bytes (Cantidad de elementos de 4 bytes) (Hay tantos elementos de 4 bytes como el valor que posee "4b Element count")
                    ...
                    ID                  2 bytes
                    Value               4 bytes
                    ...
                    8b Element count    2 bytes (Cantidad de elementos de 8 bytes) (Hay tantos elementos de 8 bytes como el valor que posee "8b Element count")
                    ...
                    ID                  2 bytes
                    Value               8 bytes
                    ...
                    Xb Element count    2 bytes (Cantidad de elementos de X bytes) (Hay tantos elementos de X bytes como el valor que posee "Xb Element count")
                    ...
                    ID                  2 bytes
                    Value               variable (Por el momento se toman 2 bytes. CONSULTAR DE DONDE OBTENER CUANTOS BYTES TRAE!!!! ????)
                    ...
                    ---
            AVL Data Count  1 byte
            */

            ClearData();

            byte[] data = (byte[])cadenaOriginal;

            //-- Header ----------------------------------------------------------------
            int bytes = 0;
            int bytes_count = 0;

            bytes = 2; 
            Length = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
            bytes_count += bytes; 
            
            bytes = 2; 
            PackedID = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
            bytes_count += bytes;

            bytes = 1; 
            PacketType = (int)getBytes(data, bytes_count, bytes, true)[0];
            bytes_count += bytes;

            bytes = 1; 
            AVLPacketID = (int)getBytes(data, bytes_count, bytes, true)[0];
            bytes_count += bytes;

            bytes = 2; 
            ImeiLength = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
            bytes_count += bytes;

            bytes = ImeiLength;
            Imei = Encoding.UTF8.GetString(getBytes((byte[])data, bytes_count, bytes, false));
            bytes_count += bytes;

            //-- Data ----------------------------------------------------------------
            bytes = 1;
            CodecID = (int)getBytes(data, bytes_count, bytes)[0];
            bytes_count += bytes;

            bytes = 1;
            AVLDataCount = (int)getBytes(data, bytes_count, bytes)[0];
            bytes_count += bytes;

            for (int i = 0; i < AVLDataCount; i++)
            {
                //-- AVL Data -------------------------------------------------------------
                AVLData AVLData_i = new AVLData();
                
                bytes = 8;
                AVLData_i.Timestamp = epoch.AddMilliseconds(BitConverter.ToInt64(getBytes(data, bytes_count, bytes, true), 0));
                bytes_count += bytes;

                bytes = 1;
                AVLData_i.Priority = (int)getBytes(data, bytes_count, bytes)[0];
                bytes_count += bytes;

                //-- GPS Element ----------------------------------------------------------
                bytes = 4;
                AVLData_i.Longitude = (float)BitConverter.ToInt32(getBytes(data, bytes_count, bytes, true), 0) / (float)10000000;
                bytes_count += bytes;

                bytes = 4;
                AVLData_i.Latitude = (float)BitConverter.ToInt32(getBytes(data, bytes_count, bytes, true), 0) / (float)10000000;
                bytes_count += bytes;

                bytes = 2;
                AVLData_i.Altitude = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                bytes_count += bytes;

                bytes = 2;
                AVLData_i.Angle = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true),0);
                bytes_count += bytes;

                bytes = 1;
                AVLData_i.Satellites = (int)getBytes(data, bytes_count, bytes)[0];
                bytes_count += bytes;

                bytes = 2;
                AVLData_i.Speed = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                bytes_count += bytes;

                //-- I/O Element ----------------------------------------------------------
                bytes = 2;
                AVLData_i.EventID = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0); 
                bytes_count += bytes;
                
                bytes = 2;
                AVLData_i.ElementCount = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0); 
                bytes_count += bytes;

                int ID;
                long Value; //8 bytes

                //-- 1 byte Elements ---------------------------------------------------
                bytes = 2;
                AVLData_i.b1ElementCount = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                bytes_count += bytes;

                for (int i_element = 0; i_element < AVLData_i.b1ElementCount; i_element++)
                {
                    bytes = 2; 
                    ID = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0); 
                    bytes_count += bytes;

                    bytes = 1; 
                    Value = (long)getBytes(data, bytes_count, bytes)[0];
                    bytes_count += bytes;

                    AVLData_i.b1ElementList.Add(new IOElement(ID, Value));
                }

                //-- 2 bytes Elements --------------------------------------------------
                bytes = 2;
                AVLData_i.b2ElementCount = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                bytes_count += bytes;

                for (int i_element = 0; i_element < AVLData_i.b2ElementCount; i_element++)
                {
                    bytes = 2;
                    ID = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0); 
                    bytes_count += bytes;

                    bytes = 2;
                    Value = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0); 
                    bytes_count += bytes;

                    AVLData_i.b2ElementList.Add(new IOElement(ID, Value));
                }

                //-- 4 bytes Elements --------------------------------------------------
                bytes = 2;
                AVLData_i.b4ElementCount = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                bytes_count += bytes;

                for (int i_element = 0; i_element < AVLData_i.b4ElementCount; i_element++)
                {
                    bytes = 2;
                    ID = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                    bytes_count += bytes;

                    bytes = 4;
                    Value = BitConverter.ToInt32(getBytes(data, bytes_count, bytes, true), 0);
                    bytes_count += bytes;

                    AVLData_i.b4ElementList.Add(new IOElement(ID, Value));
                }

                //-- 8 bytes Elements --------------------------------------------------
                bytes = 2;
                AVLData_i.b8ElementCount = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                bytes_count += bytes;

                for (int i_element = 0; i_element < AVLData_i.b8ElementCount; i_element++)
                {
                    bytes = 2;
                    ID = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                    bytes_count += bytes;

                    bytes = 8;
                    Value = BitConverter.ToInt64(getBytes(data, bytes_count, bytes, true), 0);
                    bytes_count += bytes;

                    AVLData_i.b8ElementList.Add(new IOElement(ID, Value));
                }

                //-- X bytes Elements --------------------------------------------------
                bytes = 2;
                AVLData_i.bXElementCount = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                bytes_count += bytes;

                for (int i_element = 0; i_element < AVLData_i.bXElementCount; i_element++)
                {
                    try
                    {
                        bytes = 2;
                        ID = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0);
                        bytes_count += bytes;

                        bytes = 2; // VARIABLE, Por el momento se toman 2 bytes. CONSULTAR DE DONDE OBTENER ESTE DATO !!!!!
                        Value = BitConverter.ToInt16(getBytes(data, bytes_count, bytes, true), 0); //(long)getBytes(data, bytes_count, bytes)[0]; 
                        bytes_count += bytes;

                        AVLData_i.bXElementList.Add(new IOElement(ID, Value));
                    } catch
                    {

                    }
                }

                if (ConfigurationManager.AppSettings["IPDestinoRGP"] != null)
                {
                    try
                    {
                        string aux_velocidad = "000" + Convert.ToInt32((AVLData_i.Speed * 0.621371)).ToString();
                        string datos = ">RPF"
                        + AVLData_i.Timestamp.ToUniversalTime().ToString("ddMMyyHHmmss") //FechaGPS
                        + (AVLData_i.Latitude.ToString().Replace(".", "") + "00000000").Substring(0, 8) //Latitud
                        + ("-0" + AVLData_i.Longitude.ToString().Replace("-", "").Replace(".", "") + "000000000").Substring(0, 9) //Longitud
                        + aux_velocidad.Substring(aux_velocidad.Length - 3) //Velocidad
                        + ("000" + AVLData_i.Angle.ToString()).Substring(("000" + AVLData_i.ToString()).Length - 3) //Sentido
                        + AVLData_i.Satellites.ToString() //"0"
                        + "00"  //Edad
                        + "00"  //Estado entradas
                        + "00"  //("00" + evento.ToString()).Substring(("00" + evento.ToString()).Length - 2)  //Evento
                        + ";ID=0" + Imei.Substring(3) + ";#0002;*52<";

                        UdpClient a = new UdpClient();
                        var envio = Encoding.ASCII.GetBytes(datos);
                        a.Send(envio, envio.Length, ConfigurationManager.AppSettings["IPDestinoRGP"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["PuertoDestinoRGP"]));
                        //Console.WriteLine("Reenviando: " + datos);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al reenviar dato RGP: " + ex.Message);
                    }
                }

                XGatewayBase.XGateway.EnviarPosicion(
                        CodigoGPS: Imei,
                        Latitud: AVLData_i.Latitude,
                        Longitud: AVLData_i.Longitude,
                        Velocidad: AVLData_i.Speed,
                        Heading: (short)AVLData_i.Angle,
                        Satelites: (byte)AVLData_i.Satellites,
                        FechaGPS: AVLData_i.Timestamp
                    );                

                if (i < AVLDataCount)
                    Thread.Sleep(50);                

                AVLDataList.Add(AVLData_i);
            }

            try
            {
                bytes = 1;
                AVLDataCountFinal = (int)getBytes(data, bytes_count, bytes)[0];
                bytes_count += bytes;
            } 
            catch { 

            }                

            return BitConverter.ToString((byte[])cadenaOriginal);
        }


        public byte[] getBytes(byte[] array, int start, int cantidad, bool reverse = false)
        {
            if (reverse)
                return array.Skip(start).Take(cantidad).Reverse().ToArray();
            else
                return array.Skip(start).Take(cantidad).ToArray();
        }

        public override List<string> GetGpsJsonData()
        {
            List<string> jsonList = new List<string>();
            
            float? vel_aux;            
            float? bat_aux; 
            int? ign_aux;
            float? odo_aux;
            float? comb_total_aux;
            int? rpm_aux;
            int? temp_aux;
            float? comb_aux;
            float? comb_p_aux;
            float? acel_aux;

            foreach (AVLData AVLData in AVLDataList)
            {

                vel_aux = GetFloatElementValue(AVLData, 81);
                
                bat_aux = GetFloatElementValue(AVLData, 66); 
                bat_aux = (bat_aux != null) ? bat_aux / 1000 : null; //en mV, pasar a Volts dividiendo por 1000

                ign_aux = GetIntElementValue(AVLData, 239);
                if (ign_aux != null)
                {
                    if (ign_aux > 0) { ign_aux = 1; } else { ign_aux = 0; }
                }

                odo_aux = GetFloatElementValue(AVLData, 87);
                odo_aux = (odo_aux != null) ? odo_aux / 1000 : null; //en metros, pasar a Km dividiendo por 1000

                comb_total_aux = GetFloatElementValue(AVLData, 83);
                comb_total_aux = (comb_total_aux != null) ? comb_total_aux / 10 : null; //en DecaLitros, pasar a Litros dividiendo por 10

                rpm_aux = GetIntElementValue(AVLData, 85);

                temp_aux = GetIntElementValue(AVLData, 115); //se debe dividir por 10
                temp_aux = (temp_aux != null) ? temp_aux / 10 : null; //se debe dividir por 10

                comb_aux = GetFloatElementValue(AVLData, 84);
                comb_aux = (comb_aux != null) ? comb_aux / 10 : null; //en DecaLitros, pasar a Litros dividiendo por 10

                comb_p_aux = GetFloatElementValue(AVLData, 89); //en %
                acel_aux = GetFloatElementValue(AVLData, 82); //en %

                GpsJsonData GpsJsonData = new GpsJsonData();
                GpsJsonData.SetData(
                    //paquete ---
                    trama: null,

                    //movil --
                    id_equipo_avle: null,
                    id_movil_avle: null,
                    id_equipo_mapsoft: null,
                    id_movil_mapsoft: null,

                    //gps ---
                    id_gps: Imei,
                    id_gps_prefijo: Prefijo,
                    fecha_gps_utc: AVLData.Timestamp, //UTC+0
                    fecha_gps_local: AVLData.Timestamp.ToLocalTime(), //UTC-3
                    fecha_server_local: DateTime.Now,
                    lat: AVLData.Latitude,
                    lon: AVLData.Longitude,
                    alt: AVLData.Altitude,
                    rumbo: AVLData.Angle,
                    id_evento: null,
                    vel_gps: AVLData.Speed,
                    sat: AVLData.Satellites,
                    acc: null,
                    azim: null,
                    rfid: null,
                    id_evento_extra: null,
                    hdop: null,
                    analog1: null,
                    analog2: null,
                    analog3: null,
                    analog4: null,
                    analog5: null,
                    analog6: null,
                    analog7: null,
                    analog8: null,
                    analog9: null,
                    estado_gps: null,
                    digital_in: null,

                    //canbus ---
                    vel: vel_aux,
                    bat: bat_aux,
                    ign: ign_aux,
                    odo: odo_aux, 
                    comb_total: comb_total_aux,
                    comb_total_p: null,
                    rpm: rpm_aux,
                    temp: temp_aux,
                    consumo_comb: null,
                    comb: comb_aux,
                    comb_p: comb_p_aux,
                    acel: acel_aux,
                    tmotor_func: null,
                    tmotor_func_vel: null,
                    tmotor_func_vel0: null,
                    comb_vel0: null,
                    pres_aceite: null
                    );

                jsonList.Add(GpsJsonData.GetDataJson());
            }

            return jsonList;
        }

        private float? GetFloatElementValue(AVLData AVLData, int ID)
        {
            float? r = null;
            
            for (int i = 0; i < AVLData.b1ElementList.Count; i++)
            {
                if (AVLData.b1ElementList[i].ID == ID)
                {
                    try
                    {
                        r = (float?)AVLData.b1ElementList[i].Value;
                    } catch 
                    {
                        r = null;
                    };
                    break;
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.b2ElementList.Count; i++)
                {
                    if (AVLData.b2ElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (float?)AVLData.b2ElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.b4ElementList.Count; i++)
                {
                    if (AVLData.b4ElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (float?)AVLData.b4ElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.b8ElementList.Count; i++)
                {
                    if (AVLData.b8ElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (float?)AVLData.b8ElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.bXElementList.Count; i++)
                {
                    if (AVLData.bXElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (float?)AVLData.bXElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            return r;
        }

        private int? GetIntElementValue(AVLData AVLData, int ID)
        {
            int? r = null;

            for (int i = 0; i < AVLData.b1ElementList.Count; i++)
            {
                if (AVLData.b1ElementList[i].ID == ID)
                {
                    try
                    {
                        r = (int?)AVLData.b1ElementList[i].Value;
                    }
                    catch
                    {
                        r = null;
                    };
                    break;
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.b2ElementList.Count; i++)
                {
                    if (AVLData.b2ElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (int?)AVLData.b2ElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.b4ElementList.Count; i++)
                {
                    if (AVLData.b4ElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (int?)AVLData.b4ElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.b8ElementList.Count; i++)
                {
                    if (AVLData.b8ElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (int?)AVLData.b8ElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            if (r == null)
            {
                for (int i = 0; i < AVLData.bXElementList.Count; i++)
                {
                    if (AVLData.bXElementList[i].ID == ID)
                    {
                        try
                        {
                            r = (int?)AVLData.bXElementList[i].Value;
                        }
                        catch
                        {
                            r = null;
                        };
                        break;
                    }
                }
            }

            return r;
        }



    }
}
