using mzcoreSharp.Protocolo;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parseos
{
    public class paquete
    {
        public string trama = null;
        public paquete(string trama = null)
        {
            this.trama = trama;
        }
        
        public bool EstaVacio()
        {
            return (trama == null) || (trama == "");
        }
    }

    public class movil
    {
        public string id_equipo_avle { get; set; }
        public string id_movil_avle { get; set; }
        public string id_equipo_mapsoft { get; set; }
        public string id_movil_mapsoft { get; set; }

        public movil(string id_equipo_avle = null,
            string id_movil_avle = null,
            string id_equipo_mapsoft = null,
            string id_movil_mapsoft = null)
        {
            this.id_equipo_avle = id_equipo_avle;
            this.id_movil_avle = id_movil_avle;
            this.id_equipo_mapsoft = id_equipo_mapsoft;
            this.id_movil_mapsoft = id_movil_mapsoft;
        }

        public bool EstaVacio()
        {
            return (this.id_equipo_avle == null) &&
                (this.id_movil_avle == null) &&
                (this.id_equipo_mapsoft == null) &&
                (this.id_movil_mapsoft == null);
        }
    }

    public class gps
    {
        public string id_gps { get; set; }
        public string id_gps_prefijo { get; set; }
        public string fecha_gps_utc { get; set; }
        public string fecha_gps_local { get; set; }
        public string fecha_server_local { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string alt { get; set; }
        public string rumbo { get; set; }
        public string id_evento { get; set; }
        public string vel_gps { get; set; }
        public string sat { get; set; }
        public string acc { get; set; }
        public string azim { get; set; }
        public string rfid { get; set; }
        public string id_evento_extra { get; set; }
        public string hdop { get; set; }
        public string analog1 { get; set; }
        public string analog2 { get; set; }
        public string analog3 { get; set; }
        public string analog4 { get; set; }
        public string analog5 { get; set; }
        public string analog6 { get; set; }
        public string analog7 { get; set; }
        public string analog8 { get; set; }
        public string analog9 { get; set; }
        public string estado_gps { get; set; }
        public string digital_in { get; set; }

        public gps(string id_gps = null, 
            string id_gps_prefijo = null, 
            string fecha_gps_utc = null,
            string fecha_gps_local = null, 
            string fecha_server_local = null,
            string lat = null,
            string lon = null,
            string alt = null,
            string rumbo = null,
            string id_evento = null,
            string vel_gps = null,
            string sat = null,
            string acc = null,
            string azim = null,
            string rfid = null,
            string id_evento_extra = null,
            string hdop = null,
            string analog1 = null,
            string analog2 = null,
            string analog3 = null,
            string analog4 = null,
            string analog5 = null,
            string analog6 = null,
            string analog7 = null,
            string analog8 = null,
            string analog9 = null,
            string estado_gps = null,
            string digital_in = null)
        {
            this.id_gps = id_gps;
            this.id_gps_prefijo = id_gps_prefijo;
            this.fecha_gps_utc = fecha_gps_utc;
            this.fecha_gps_local = fecha_gps_local;
            this.fecha_server_local = fecha_server_local;
            this.lat = lat;
            this.lon = lon;
            this.alt = alt;
            this.rumbo = rumbo;
            this.id_evento = id_evento;
            this.vel_gps = vel_gps;
            this.sat = sat;
            this.acc = acc;
            this.azim = azim;
            this.rfid = rfid;
            this.id_evento_extra = id_evento_extra;
            this.hdop = hdop;
            this.analog1 = analog1;
            this.analog2 = analog2;
            this.analog3 = analog3;
            this.analog4 = analog4;
            this.analog5 = analog5;
            this.analog6 = analog6;
            this.analog7 = analog7;
            this.analog8 = analog8;
            this.analog9 = analog9;
            this.estado_gps = estado_gps;
            this.digital_in = digital_in;
        }

        public bool EstaVacio()
        {
            return (this.id_gps == null) &&
                (this.id_gps_prefijo == null) &&
                (this.fecha_gps_utc == null) &&
                (this.fecha_gps_local == null) &&
                (this.fecha_server_local == null) &&
                (this.lat == null) &&
                (this.lon == null) &&
                (this.alt == null) &&
                (this.rumbo == null) &&
                (this.id_evento == null) &&
                (this.vel_gps == null) &&
                (this.sat == null) &&
                (this.acc == null) &&
                (this.azim == null) &&
                (this.rfid == null) &&
                (this.id_evento_extra == null) &&
                (this.hdop == null) &&
                (this.analog1 == null) &&
                (this.analog2 == null) &&
                (this.analog3 == null) &&
                (this.analog4 == null) &&
                (this.analog5 == null) &&
                (this.analog6 == null) &&
                (this.analog7 == null) &&
                (this.analog8 == null) &&
                (this.analog9 == null) &&
                (this.estado_gps == null) &&
                (this.digital_in == null);
        }
    }

    public class canbus
    {
        public string vel { get; set; }
        public string bat { get; set; }
        public string ign { get; set; }
        public string odo { get; set; }
        public string comb_total { get; set; }
        public string comb_total_p { get; set; }
        public string rpm { get; set; }
        public string temp { get; set; }
        public string consumo_comb { get; set; }
        public string comb { get; set; }
        public string comb_p { get; set; }
        public string acel { get; set; }
        public string tmotor_func { get; set; }
        public string tmotor_func_vel { get; set; }
        public string tmotor_func_vel0 { get; set; }
        public string comb_vel0 { get; set; }
        public string pres_aceite { get; set; }

        public canbus(string vel, 
            string bat, 
            string ign, 
            string odo, 
            string comb_total, 
            string comb_total_p, 
            string rpm, 
            string temp, 
            string consumo_comb, 
            string comb, 
            string comb_p, 
            string acel, 
            string tmotor_func, 
            string tmotor_func_vel, 
            string tmotor_func_vel0, 
            string comb_vel0,
            string pres_aceite)
        {
            this.vel = vel;
            this.bat = bat;
            this.ign = ign;
            this.odo = odo;
            this.comb_total = comb_total;
            this.comb_total_p = comb_total_p;
            this.rpm = rpm;
            this.temp = temp;
            this.consumo_comb = consumo_comb;
            this.comb = comb;
            this.comb_p = comb_p;
            this.acel = acel;
            this.tmotor_func = tmotor_func;
            this.tmotor_func_vel = tmotor_func_vel;
            this.tmotor_func_vel0 = tmotor_func_vel0;
            this.comb_vel0 = comb_vel0;
            this.pres_aceite = pres_aceite;
        }

        public bool EstaVacio()
        {
            return (this.vel == null) &&
                (this.bat == null) &&
                (this.ign == null) &&
                (this.odo == null) &&
                (this.comb_total == null) &&
                (this.comb_total_p == null) &&
                (this.rpm == null) &&
                (this.temp == null) &&
                (this.consumo_comb == null) &&
                (this.comb == null) &&
                (this.comb_p == null) &&
                (this.acel == null) &&
                (this.tmotor_func == null) &&
                (this.tmotor_func_vel == null) &&
                (this.tmotor_func_vel0 == null) &&
                (this.comb_vel0 == null) &&
                (this.pres_aceite == null);
        }
    }

    public class GpsJsonData
    {
        public paquete paquete { get; set; }
        public movil movil { get; set; }
        public gps gps { get; set; }
        public canbus canbus { get; set; }

        public void SetData(
            //paquete ---
            string trama = null,

            //movil ---
            int? id_equipo_avle = null,
            int? id_movil_avle = null,
            int? id_equipo_mapsoft = null,
            int? id_movil_mapsoft = null,

            //gps ---
            string id_gps = null, 
            string id_gps_prefijo = null, 
            DateTime? fecha_gps_utc = null,
            DateTime? fecha_gps_local = null,
            DateTime? fecha_server_local = null,
            float? lat = null,
            float? lon = null,
            float? alt = null,
            int? rumbo = null,
            int? id_evento = null,
            float? vel_gps = null,
            int? sat = null,
            float? acc = null,
            int? azim = null,
            string rfid = null,
            int? id_evento_extra = null,
            float? hdop = null,
            float? analog1 = null,
            float? analog2 = null,
            float? analog3 = null,
            float? analog4 = null,
            float? analog5 = null,
            float? analog6 = null,
            float? analog7 = null,
            float? analog8 = null,
            float? analog9 = null,
            float? estado_gps = null,
            int? digital_in = null,

            //canbus --
            float? vel = null,
            float? bat = null,
            int? ign = null,
            float? odo = null,
            float? comb_total = null,
            float? comb_total_p = null,
            int? rpm = null,
            int? temp = null,
            float? consumo_comb = null,
            float? comb = null,
            float? comb_p = null,
            float? acel = null,
            float? tmotor_func = null,
            float? tmotor_func_vel = null,
            float? tmotor_func_vel0 = null,
            float? comb_vel0 = null,
            float? pres_aceite = null
            )
        {
            this.paquete = new paquete(trama);
            if (this.paquete.EstaVacio()) { this.paquete = null; };

            this.movil = new movil(id_equipo_avle?.ToString(),
                id_movil_avle?.ToString(),
                id_equipo_mapsoft?.ToString(),
                id_movil_mapsoft?.ToString());
            if (this.movil.EstaVacio()) { this.movil = null; };

            this.gps = new gps(id_gps,
                               id_gps_prefijo,
                               fecha_gps_utc?.ToString("yyyyMMddHHmmss"),
                               fecha_gps_local?.ToString("yyyyMMddHHmmss"),
                               fecha_server_local?.ToString("yyyyMMddHHmmss"),
                               lat?.ToString(CultureInfo.InvariantCulture),
                               lon?.ToString(CultureInfo.InvariantCulture),
                               alt?.ToString(CultureInfo.InvariantCulture),
                               rumbo?.ToString(),
                               id_evento?.ToString(),
                               vel_gps?.ToString(CultureInfo.InvariantCulture),
                               sat?.ToString(),
                               acc?.ToString(),
                               azim?.ToString(),
                               rfid?.ToString(),
                               id_evento_extra?.ToString(),
                               hdop?.ToString(),
                               analog1?.ToString(CultureInfo.InvariantCulture),
                               analog2?.ToString(CultureInfo.InvariantCulture),
                               analog3?.ToString(CultureInfo.InvariantCulture),
                               analog4?.ToString(CultureInfo.InvariantCulture),
                               analog5?.ToString(CultureInfo.InvariantCulture),
                               analog6?.ToString(CultureInfo.InvariantCulture),
                               analog7?.ToString(CultureInfo.InvariantCulture),
                               analog8?.ToString(CultureInfo.InvariantCulture),
                               analog9?.ToString(CultureInfo.InvariantCulture),
                               estado_gps?.ToString(),
                               digital_in?.ToString()
                               );
            if (this.gps.EstaVacio()) { this.gps = null; };

            this.canbus = new canbus(vel?.ToString(CultureInfo.InvariantCulture),
                bat?.ToString(CultureInfo.InvariantCulture),
                ign?.ToString(),
                odo?.ToString(CultureInfo.InvariantCulture),
                comb_total?.ToString(CultureInfo.InvariantCulture),
                comb_total_p?.ToString(CultureInfo.InvariantCulture),
                rpm?.ToString(),
                temp?.ToString(),
                consumo_comb?.ToString(CultureInfo.InvariantCulture),
                comb?.ToString(CultureInfo.InvariantCulture),
                comb_p?.ToString(CultureInfo.InvariantCulture),
                acel?.ToString(CultureInfo.InvariantCulture),
                tmotor_func?.ToString(CultureInfo.InvariantCulture),
                tmotor_func_vel?.ToString(CultureInfo.InvariantCulture),
                tmotor_func_vel0?.ToString(CultureInfo.InvariantCulture),
                comb_vel0?.ToString(CultureInfo.InvariantCulture),
                pres_aceite?.ToString(CultureInfo.InvariantCulture)
                );
            if (this.canbus.EstaVacio()) { this.canbus = null; };
        }

        public string GetDataJson()
        {
            var opciones = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };

            return JsonSerializer.Serialize(this, opciones);
        }
    }
}
