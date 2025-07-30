using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using mzcoreSharp.Protocolo;
using System.Data;
using System.Runtime.Remoting.Lifetime;
//using mzcoreSharp.CORE;
//using System.Web.Services.Protocols;
//using System.Windows.Forms.VisualStyles;
//using System.Web.Security.AntiXss;
//using System.Drawing;

namespace Parseos
{
    class GL300 : Parser
    {
        public static Dictionary<string, int> Eventos = new Dictionary<string, int>();

        private static class can_report_mask_bits
        {
            public const byte CELL_INFORMATION = 31; //mcc, mnc, lac, cell_id, reserved="00 
            public const byte GNSS_INFORMATION = 30; //gnss_accuracy, gnss_speed, azimuth, altitude, longitude, latitude, gnss_utc_time
            public const byte CAN_REPORT_EXPANSION_MASK = 29;
            public const byte RESERVED_28 = 28;
            public const byte RESERVED_27 = 27;
            public const byte RESERVED_26 = 26;
            public const byte RESERVED_25 = 25;
            public const byte RESERVED_24 = 24;
            public const byte RESERVED_23 = 23;
            public const byte TOTAL_DISTANCE_IMPULSES = 22; //total_distance
            public const byte TOTAL_VEHICLE_ENGINE_OVERSPEED_TIME = 21; 
            public const byte TOTAL_VEHICLE_OVERSPEED_TIME = 20; 
            public const byte DOORS = 19; 
            public const byte LIGHTS = 18; 
            public const byte DETAILED_INFORMATION = 17;
            public const byte TACHOGRAPH_INFORMATION = 16;
            public const byte AXLE_WEIGHT_2ND = 15;
            public const byte TOTAL_IDLE_FUEL_USED = 14;
            public const byte TOTAL_ENGINE_IDLE_TIME = 13;
            public const byte TOTAL_DRIVING_TIME = 12;
            public const byte TOTAL_ENGINE_HOURS = 11;
            public const byte ACCELERATOR_PEDAL_PRESSURE = 10;
            public const byte RANGE = 9;
            public const byte FUEL_LEVEL = 8;
            public const byte FUEL_CONSUMPTION = 7;
            public const byte ENGINE_COOLANT_TEMPERATURE = 6;
            public const byte ENGINE_RPM = 5;
            public const byte VEHICLE_SPEED = 4;
            public const byte TOTAL_FUEL_USED = 3;
            public const byte TOTAL_DISTANCE = 2;
            public const byte IGNITION_KEY = 1;
            public const byte VIN = 0;
        }

        private static class can_expand_report_mask_bits
        {
            public const byte RESERVED_31 = 31; 
            public const byte RESERVED_30 = 30;
            public const byte RESERVED_29 = 29;
            public const byte RESERVED_28 = 28;
            public const byte RESERVED_27 = 27;
            public const byte RESERVED_26 = 26;
            public const byte RESERVED_25 = 25;
            public const byte SERVICE_DISTANCE = 24;
            public const byte RESERVED_23 = 23;
            public const byte RAPID_ACCELERATIONS = 22;
            public const byte RAPID_BRAKINGS = 21;
            public const byte EXPANDED_INFORMATION = 20;
            public const byte REGISTRATION_NUMBER = 19;
            public const byte TACHOGRAPH_DRIVER_2_NAME = 18;
            public const byte TACHOGRAPH_DRIVER_1_NAME = 17;
            public const byte TACHOGRAPH_DRIVER_2_CARD_NUMBER = 16;
            public const byte TACHOGRAPH_DRIVER_1_CARD_NUMBER = 15;
            public const byte TOTAL_BRAKES_APPLICATIONS = 14;
            public const byte TOTAL_ACCELERATOR_KICK_DOWN_TIME = 13;
            public const byte TOTAL_CRUISE_CONTROL_TIME = 12;
            public const byte TOTAL_EFFECTIVE_ENGINE_SPEED_TIME = 11;
            public const byte TOTAL_ACCELERATOR_KICK_DOWNS = 10;
            public const byte PEDAL_BRAKING_FACTOR = 9;
            public const byte ENGINE_BRAKING_FACTOR = 8;
            public const byte ANALOG_INPUT_VALUE = 7;
            public const byte TACHOGRAHP_DRIVING_DIRECTION = 6;
            public const byte TACHOGRAHP_VEHICLE_MOTION_SIGNAL = 5;
            public const byte TACHOGRAHP_OVERSPEED_SIGNAL = 4;
            public const byte AXLE_WEIGTH_4TH = 3;
            public const byte AXLE_WEIGTH_3RD = 2;
            public const byte AXLE_WEIGTH_1ST = 1;
            public const byte AD_BLUE_LEVEL = 0;
        }

        private static class eri_mask_bits
        {
            public const byte RF433_ACCESORY_DATA = 7;
            public const byte RESERVED_6 = 6;
            public const byte RESERVED_5 = 5;
            public const byte FUEL_SENSOR_DATA_VOLUME = 4; //Bloque de datos: Fuel Sensor Data (Volumen)
            public const byte FUEL_SENSOR_DATA_PERCENTAGE = 3; //Bloque de datos: Fuel Sensor Data (Porcentaje)
            public const byte CAN_DATA = 2;
            public const byte _1_WIRE_DATA = 1;
            public const byte DIGITAL_FUEL_SENSOR_DATA = 0;
        }

        private static string Prefijo = "";
        private static string codigoGPS = "";
        private static string evento = "";
        private static int? numero_evento = null;

        private static float? HDOP = null;                         //Horizontal Dilution of Precision
        private static float? velocidad = null;
        private static short? heading = null;
        private static float? longitud = null;
        private static float? latitud = null;
        private static DateTime? fechaGPS = null;
        private static float? Bateria = null;
        private static DateTime? fechaUtc = null;

        //Canbus ------------------------------------
        private static string protocol_version = null;
        private static string unique_id = null;
        private static string device_name = null;
        private static int? report_type = null;                            //Tipo de reporte (0: reporte periodico, 1: pedido de reporte en tiempo real, 2 reporte de ignicion on/off)
        private static int? canbus_device_state = null;                    //Estado de comunicacion con el dispositivo canbus externo (0: Anormal-falla para recibir datos, 1: Normal-habilitado para recibir datos)
        private static string canbus_report_mask = null;                   //Consultar <CAN Report Mask> en AT+GTCAN
        private static string canbus_report_mask_bin = null;               //CAN Report Mask en Binario (para manejo interno)
        private static string vin = null;                                  //Reservado en dispositivos CAN100
        private static int? ignition_key = null;                           //Estado de la ingnición (0: ignición off, 1: ignición on, 2: motor on)
        private static float? total_distance = null;                       //Distancia total recorrida por el vehiculo ("H" Hectometros ó "I" Impulso de Distancia, si la distancia no esta disponible en el panel)
        private static string total_distance_units = null;                 //Dato adicional local. Unidades de la distancia total recorrida del vehiculo ("H" ó "I")               
        private static float? total_fuel_used = null;                      //Cantidad de combustible utilizado desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades en Litros.
        private static int? engine_rpm = null;                             //Revoluciones por minuto del motor
        private static float? vehicle_speed = null;                        //Velocidad del vehiculo. Unidades Km/h
        private static int? engine_coolant_temperature = null;             //Temperatura de motor. Unidades ºC 
        private static float? fuel_consumption = null;                     //Consumo de combustible. Unidades Litros/100km
        private static float? fuel_level = null;                           //Nivel de combustible en el tanque. Unidades "L" Litros ó "P" Porcentaje
        private static string fuel_level_units = null;                     //Dato adicional local. Unidades del nivel de combustible.
        private static float? range = null;                                //Cantidad de hectometros a recorrer con el combustible restante. Unidades "H" Hectometros
        private static string range_units = null;                          //Dato adicional local. Unidades de range ("H" = Hectometros)
        private static float? accelerator_pedal_pressure = null;             //Presion del acelerados. Unidades % Porcentaje
        private static float? total_engine_hours = null;                   //Tiempo total de motor funcionando desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
        private static float? total_driving_time = null;                   //Tiempo total de motor funcionando en velocidad mayor a cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
        private static float? total_engine_idle_time = null;               //Tiempo total de motor funcionando en velocidad cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
        private static float? total_idle_fuel_used = null;                 //Cantidad de combustible utilizado con motor funcionando en velocidad cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Litros.
        private static int? axle_weight_2nd = null;                        //Peso del segundo eje del vehiculo. Unidades Kg
        private static string tachograph_information = null;               //Dos bytes. El byte alto describe el tacografo del driver 2 y el byte bajo describe el el tacografo del driver 1
        /*Formato de ambos bytes de tachograph_information: V - R - W1 - W0 - C - T2 - T1 - T0
          V: Validacion (0: dato valido, 1: dato invalido)
          R: Reservado
          C: Estado de la tarjeta del driver (1: tarjeta insertada, 0: tarjeta no insertada)
          T2-T0: Estado relacionado al tiempo de conducción
            0: Normal (sin limites alcanzados)
            1: 15min antes de las 4.5 horas
            2: 4.5 horas alcanzadas
            3: 15min antes de las 9 horas
            4: 9 horas alcanzadas
            5: 15min antes de las 16 horas
            6: 16 horas alcanzadas
            7: otro limite
          W1-W0: Estado de trabajo del driver
            0: Normal (sin limites alcanzados)
            1: Descanso - Durmiendo
            2: Driver habilitado - Breve descanso
            3: Drive - Detras del volante
        */
        private static string detailed_information = null;                //Numero hexadecimal. Cada bit contiene información de un indicador
        /*Bit 0: FL – Indicador de bajo combustible (1: on, 0: off)
          Bit 1: DS – Indicador de cinturon de seguridad del conductor (1: on, 0: off)
          Bit 2: AC – Aire acondicionado (1: on, 0: off)
          Bit 3: CC – Control de crucero (1: on, 0: off)
          Bit 4: B – Pedal de freno (1: presionado, 0: liberado)
          Bit 5: C – Pedal de embrague (1: presionado, 0: liberado)
          Bit 6: H – Freno de mano (1: levantado, 0: liberado)
          Bit 7: CL – Cierre centralizado (1: bloqueado, 0: desbloqueado)
          Bit 8: R – Reversa (1: acoplada, 0: desacoplada)
          Bit 9: RL – Luces de marcha (1: on, 0: off)
          Bit 10: LB – Luces bajas (1: on, 0: off)
          Bit 11: HB – Luces altas (1: on, 0: off)
          Bit 12: RFL – Luces antiniebla traseras (1: on, 0: off)
          Bit 13: FFL – Luces antiniebla delateras (1: on, 0: off)
          Bit 14: D – Puertas (1: alguna puerta abierta, 0: todas las puertas cerradas)
          Bit 15: T – Baul (1: abierto, 0: cerrado)
         */
        private static string lights = null;                              //Numero hexadecimal. Cada bit contiene información de una luz particular
        /*Bit 0: Luces de circulación diurna (1: encendidas, 0:  apagadas)
          Bit 1: Luces bajas (1: encendidas, 0:  apagadas)
          Bit 2: Luces altas (1: encendidas, 0: apagadas)
          Bit 3: Luz antiniebla delantera (1: encendida, 0: apagada)
          Bit 4: Luz antiniebla trasera (1: encendida, 0: apagada)
          Bit 5: Balizas (1: encendidas, 0: apagadas)
          Bit 6: Reservado
          Bit 7: Reservado
        */
        private static string doors = null;                               //Numero hexadecimal. Cada bit contiene información de una puerta particular
        /*Bit 0: Puerta del conductor (1: abierta, 0: cerrada)
          Bit 1: Puerta del pasajero (1: abierta, 0: cerrada)
          Bit 2: Puerta trasera izquierda (1: abierta, 0: cerrada)
          Bit 3: Puerta trasera derecha (1: abierta, 0: cerrada)
          Bit 4: Baul (1: abierto, 0: cerrado)
          Bit 5: Capó (1: abierto, 0: cerrado)
          Bit 6: Reservado
          Bit 7: Reservado 
        */
        private static float? total_vehicle_overspeed_time = null;        //Tiempo total cuando la velocidad del vehiculo es mayor al limite definido en la configuracion CAN100
        private static float? total_vehicle_engine_overspeed_time = null; //Tiempo total cuando la velocidad del motor es mayor al limite definido en la configuracion CAN100
        private static string can_expand_report_mask = null;
        private static string can_expand_report_mask_bin = null;
        private static float? ad_blue_level = null;
        private static float? axle_weigth_1st = null;
        private static float? axle_weigth_3rd = null;
        private static float? axle_weigth_4th = null;
        private static int? tachograhp_overspeed_signal = null;
        private static int? tachograhp_vehicle_motion_signal = null;
        private static int? tachograhp_driving_direction = null;
        private static int? analog_input_value = null;
        private static float? engine_braking_factor = null;
        private static float? pedal_braking_factor = null;
        private static float? total_accelerator_kick_downs = null;
        private static float? total_effective_engine_speed_time = null;
        private static float? total_cruise_control_time = null;
        private static float? total_accelerator_kick_down_time = null;
        private static int? total_brakes_applications = null;
        private static int? service_distance = null;
        private static string tachograph_driver_1_card_number = null;
        private static string tachograph_driver_2_card_number = null;
        private static string tachograph_driver_1_name = null;
        private static string tachograph_driver_2_name = null;
        private static string registration_number = null;
        private static string expanded_information = null;
        private static int? rapid_brakings = null;
        private static int? rapid_accelerations = null;
        private static string reserved1 = null;
        private static string reserved2 = null;
        private static int? gnss_accuracy = null;
        private static float? gnss_speed = null;
        private static int? azimuth = null;
        private static float? altitude = null;                             //Altura sobre el nivel del mar. En metros
        private static float? longitude = null;
        private static float? latitude = null;
        private static DateTime? gnss_utc_time = null;                     //GNSS=Global Navigation Satellite System; UTC=Coordinated Universal Time
        private static string mcc = null;
        private static string mnc = null;
        private static string lac = null;
        private static string cell_id = null;
        private static string position_append_mask = null;
        private static int? satellites_in_view = null;
        private static DateTime? send_time = null;
        private static string count_number = null;
        private static string tail_character = null;
        //------------------------------------------

        //GTERI ------------------------------------
        private static string eri_mask = null;
        private static string eri_mask_bin = null;
        private static int? external_power_supply = null;
        private static int? report_id = null;
        private static int? number = null;

        //------------------------------------------
        public GL300(string prefijo = null)
        {
            Eventos.Add("GTPNA", 1); //Encendido
            Eventos.Add("GTPFA", 2); //Apagado
            Eventos.Add("GTEPN", 3); //Bateria conectada
            Eventos.Add("GTEPF", 4); //Bateria desconectada
            Eventos.Add("GTBPL", 5); //Bateria baja
            Eventos.Add("GTBTC", 6); //Bateria inicio carga 
            Eventos.Add("GTSTC", 7); //Bateria detencion carga
            Eventos.Add("GTIGN", 8); //IGN ON
            Eventos.Add("GTIGF", 9); //IGN OFF
            Eventos.Add("GTUPC", 10); //Comando recibido
            Eventos.Add("GTSOS", 11); //Panico
            Eventos.Add("GTCAN", 12); //CanBus
            Eventos.Add("GTFRI", 13); //
            Eventos.Add("GTERI", 14); //Datos + CanBus
            Eventos.Add("GTSTT", 15); //Status motion
            Eventos.Add("GTHBM", 16); //Hash behavior
            Eventos.Add("GTDOG", 17); //
            Eventos.Add("GTPDP", 18); //
            

            Prefijo = prefijo ?? "";
            PaqueteEnString = true;
        }

        public override object LimpiarPaquete(object cadenaOriginal)
        {
            ParseData = "";

            codigoGPS = "";
            evento = "";
            numero_evento = null;
            HDOP = null;
            velocidad = null;
            heading = null;
            longitud = null;
            latitud = null;
            fechaGPS = null;
            Bateria = null;

            protocol_version = null;
            unique_id = null;
            device_name = null;
            report_type = null;
            canbus_device_state = null;
            canbus_report_mask = null;
            canbus_report_mask_bin = null;
            vin = null;
            ignition_key = null;
            total_distance = null;
            total_distance_units = null;
            total_fuel_used = null;
            engine_rpm = null;
            vehicle_speed = null;
            engine_coolant_temperature = null;
            fuel_consumption = null;
            fuel_level = null;
            fuel_level_units = null;
            range = null;
            range_units = null;
            accelerator_pedal_pressure = null;
            total_engine_hours = null;
            total_driving_time = null;
            total_engine_idle_time = null;
            total_idle_fuel_used = null;
            axle_weight_2nd = null;
            tachograph_information = null;
            detailed_information = null;
            lights = null;
            doors = null;
            total_vehicle_overspeed_time = null;
            total_vehicle_engine_overspeed_time = null;
            can_expand_report_mask = null;
            can_expand_report_mask_bin = null;
            ad_blue_level = null;
            axle_weigth_1st = null;
            axle_weigth_3rd = null;
            axle_weigth_4th = null;
            tachograhp_overspeed_signal = null;
            tachograhp_vehicle_motion_signal = null;
            tachograhp_driving_direction = null;
            analog_input_value = null;
            engine_braking_factor = null;
            pedal_braking_factor = null;
            total_accelerator_kick_downs = null;
            total_effective_engine_speed_time = null;
            total_cruise_control_time = null;
            total_accelerator_kick_down_time = null;
            total_brakes_applications = null;
            service_distance = null;
            tachograph_driver_1_card_number = null;
            tachograph_driver_2_card_number = null;
            tachograph_driver_1_name = null;
            tachograph_driver_2_name = null;
            registration_number = null;
            expanded_information = null;
            rapid_brakings = null;
            rapid_accelerations = null;
            reserved1 = null;
            reserved2 = null;
            gnss_accuracy = null;
            gnss_speed = null;
            azimuth = null;
            altitude = null;
            longitude = null;
            latitude = null;
            gnss_utc_time = null;
            mcc = null;
            mnc = null;
            lac = null;
            cell_id = null;
            position_append_mask = null;
            satellites_in_view = null;
            send_time = null;
            count_number = null;
            tail_character = null;

            eri_mask = null;
            eri_mask_bin = null;
            external_power_supply = null;
            report_id = null;
            number = null;

            string cadena = cadenaOriginal.ToString().Trim();
            return cadena;
        }

        public override string getParseData()
        {
            string r = "";

            r += "evento=" + evento + " ";
            r += "numero_evento=" + numero_evento + " ";

            r += "fechaGPS=" + fechaGPS.ToString() + " ";
            r += "velocidad=" + velocidad.ToString() + " ";
            r += "heading=" + heading.ToString() + " ";
            r += "latitud=" + latitud.ToString() + " ";
            r += "longitud=" + longitud.ToString() + " ";

            switch (evento)
            {
                case "GTCAN":
                    r += "\r\n";// + "--- Can Bus ---" + "\r\n";
                    if (protocol_version != null) { r += "protocol_version=" + protocol_version.ToString() + " "; }
                    if (unique_id != null) { r += "unique_id=" + unique_id.ToString() + " "; }
                    if (device_name != null) { r += "device_name=" + device_name.ToString() + " "; }
                    if (report_type != null) { r += "report_type=" + report_type.ToString() + " "; }
                    if (canbus_device_state != null) { r += "canbus_device_state=" + canbus_device_state.ToString() + " "; }
                    if (canbus_report_mask != null) { r += "canbus_report_mask=" + canbus_report_mask.ToString() + " "; }
                    if (vin != null) { r += "vin=" + vin.ToString() + " "; }
                    if (ignition_key != null) { r += "ignition_key=" + ignition_key.ToString() + " "; }
                    if (total_distance != null) { r += "total_distance=" + total_distance.ToString() + " "; }
                    if (total_distance != null) { r += "total_distance_units=" + total_distance_units.ToString() + " "; }
                    if (total_fuel_used != null) { r += "total_fuel_used=" + total_fuel_used.ToString() + " "; }
                    if (engine_rpm != null) { r += "engine_rpm=" + engine_rpm.ToString() + " "; }
                    if (vehicle_speed != null) { r += "vehicle_speed=" + vehicle_speed.ToString() + " "; }
                    if (engine_coolant_temperature != null) { r += "engine_coolant_temperature=" + engine_coolant_temperature.ToString() + " "; }
                    if (fuel_consumption != null) { r += "fuel_consumption=" + fuel_consumption.ToString() + " "; }
                    if (fuel_level != null) { r += "fuel_level=" + fuel_level.ToString() + " "; }
                    if (fuel_level != null) { r += "fuel_level_units=" + fuel_level_units.ToString() + " "; }
                    if (range != null)                      { r += "range=" + range.ToString() + " "; }
                    if (range != null)                      { r += "range_units=" + range_units.ToString() + " "; }
                    if (accelerator_pedal_pressure != null) { r += "accelerator_pedal_pressure=" + accelerator_pedal_pressure.ToString() + " "; }
                    if (total_engine_hours != null) { r += "total_engine_hours=" + total_engine_hours.ToString() + " "; }
                    if (total_driving_time != null) { r += "total_driving_time=" + total_driving_time.ToString() + " "; }
                    if (total_engine_idle_time != null) { r += "total_engine_idle_time=" + total_engine_idle_time.ToString() + " "; }
                    if (total_idle_fuel_used != null) { r += "total_idle_fuel_used=" + total_idle_fuel_used.ToString() + " "; }
                    if (axle_weight_2nd != null) { r += "axle_weight_2nd=" + axle_weight_2nd.ToString() + " "; }
                    if (tachograph_information != null)     { r += "tachograph_information=" + tachograph_information.ToString() + " "; }
                    if (detailed_information != null) { r += "detailed_information=" + detailed_information.ToString() + " "; }
                    if (lights != null)                     { r += "lights=" + lights.ToString() + " "; }
                    if (doors != null) { r += "doors=" + doors.ToString() + " "; }
                    if (total_vehicle_overspeed_time != null)           { r += "total_vehicle_overspeed_time=" + total_vehicle_overspeed_time.ToString() + " "; }
                    if (total_vehicle_engine_overspeed_time != null)    { r += "total_vehicle_engine_overspeed_time=" + total_vehicle_engine_overspeed_time.ToString() + " "; }
                    if (can_expand_report_mask != null) { r += "can_expand_report_mask=" + can_expand_report_mask.ToString() + " "; }
                    if (ad_blue_level != null)                          { r += "ad_blue_level=" + ad_blue_level.ToString() + " "; }
                    if (axle_weigth_1st != null)                        { r += "axle_weigth_1st=" + axle_weigth_1st.ToString() + " "; }
                    if (axle_weigth_3rd != null)                        { r += "axle_weigth_3rd=" + axle_weigth_3rd.ToString() + " "; }
                    if (axle_weigth_4th != null)                        { r += "axle_weigth_4th=" + axle_weigth_4th.ToString() + " "; }
                    if (tachograhp_overspeed_signal != null)            { r += "tachograhp_overspeed_signal=" + tachograhp_overspeed_signal.ToString() + " "; }
                    if (tachograhp_vehicle_motion_signal != null)       { r += "tachograhp_vehicle_motion_signal=" + tachograhp_vehicle_motion_signal.ToString() + " "; }
                    if (tachograhp_driving_direction != null)           { r += "tachograhp_driving_direction=" + tachograhp_driving_direction.ToString() + " "; }
                    if (analog_input_value != null)                     { r += "analog_input_value=" + analog_input_value.ToString() + " "; }
                    if (engine_braking_factor != null)                  { r += "engine_braking_factor=" + engine_braking_factor.ToString() + " "; }
                    if (pedal_braking_factor != null)                   { r += "pedal_braking_factor=" + pedal_braking_factor.ToString() + " "; }
                    if (total_accelerator_kick_downs != null)           { r += "total_accelerator_kick_downs=" + total_accelerator_kick_downs.ToString() + " "; }
                    if (total_effective_engine_speed_time != null)      { r += "total_effective_engine_speed_time=" + total_effective_engine_speed_time.ToString() + " "; }
                    if (total_cruise_control_time != null)              { r += "total_cruise_control_time=" + total_cruise_control_time.ToString() + " "; }
                    if (total_accelerator_kick_down_time != null)       { r += "total_accelerator_kick_down_time=" + total_accelerator_kick_down_time.ToString() + " "; }
                    if (total_brakes_applications != null)              { r += "total_brakes_applications=" + total_brakes_applications.ToString() + " "; }
                    if (service_distance != null)                       { r += "service_distance=" + service_distance.ToString() + " "; }
                    if (tachograph_driver_1_card_number != null)        { r += "tachograph_driver_1_card_number=" + tachograph_driver_1_card_number.ToString() + " "; }
                    if (tachograph_driver_2_card_number != null)        { r += "tachograph_driver_2_card_number=" + tachograph_driver_2_card_number.ToString() + " "; }
                    if (tachograph_driver_1_name != null)               { r += "tachograph_driver_1_name=" + tachograph_driver_1_name.ToString() + " "; }
                    if (tachograph_driver_2_name != null)               { r += "tachograph_driver_2_name=" + tachograph_driver_2_name.ToString() + " "; }
                    if (registration_number != null)                    { r += "registration_number=" + registration_number.ToString() + " "; }
                    if (expanded_information != null)                   { r += "expanded_information=" + expanded_information.ToString() + " "; }
                    if (rapid_brakings != null)                         { r += "rapid_brakings=" + rapid_brakings.ToString() + " "; }
                    if (rapid_accelerations != null)                    { r += "rapid_accelerations=" + rapid_accelerations.ToString() + " "; }
                    if (reserved1 != null)                              { r += "reserved1=" + reserved1.ToString() + " "; }
                    if (reserved2 != null)                              { r += "reserved2=" + reserved2.ToString() + " "; }
                    if (gnss_accuracy != null) { r += "gnss_accuracy=" + gnss_accuracy.ToString() + " "; }
                    if (gnss_speed != null) { r += "gnss_speed=" + gnss_speed.ToString() + " "; }
                    if (azimuth != null) { r += "azimuth=" + azimuth.ToString() + " "; }
                    if (altitude != null) { r += "altitude=" + altitude.ToString() + " "; }
                    if (longitude != null) { r += "longitude=" + longitude.ToString() + " "; }
                    if (latitude != null) { r += "latitude=" + latitude.ToString() + " "; }
                    if (gnss_utc_time != null) { r += "gnss_utc_time=" + gnss_utc_time.ToString() + " "; }
                    if (mcc != null)                                    { r += "mcc=" + mcc.ToString() + " "; }
                    if (mnc != null)                                    { r += "mnc=" + mnc.ToString() + " "; }
                    if (lac != null)                                    { r += "lac=" + lac.ToString() + " "; }
                    if (cell_id != null)                                { r += "cell_id=" + cell_id.ToString() + " "; }
                    if (position_append_mask != null)                   { r += "position_append_mask=" + position_append_mask.ToString() + " "; }
                    if (satellites_in_view != null) { r += "satellites_in_view=" + satellites_in_view.ToString() + " "; }
                    if (send_time != null) { r += "send_time=" + send_time.ToString() + " "; }
                    if (count_number != null) { r += "count_number=" + count_number.ToString() + " "; }
                    if (tail_character != null) { r += "tail_character=" + tail_character.ToString() + " "; }
                    break;

                case "GTERI":
                    r += "\r\n";
                    if (protocol_version != null) { r += "protocol_version=" + protocol_version.ToString() + " "; }
                    if (unique_id != null) { r += "unique_id=" + unique_id.ToString() + " "; }
                    if (device_name != null) { r += "device_name=" + device_name.ToString() + " "; }
                    if (eri_mask != null) { r += "eri_mask=" + eri_mask.ToString() + " "; }
                    if (external_power_supply != null) { r += "external_power_supply=" + external_power_supply.ToString() + " "; }
                    if (report_id != null) { r += "report_id=" + report_id.ToString() + " "; }
                    if (number != null) { r += "number=" + number.ToString() + " "; }
                    if (gnss_accuracy != null) { r += "gnss_accuracy=" + gnss_accuracy.ToString() + " "; }
                    if (gnss_speed != null) { r += "gnss_speed=" + gnss_speed.ToString() + " "; }
                    if (azimuth != null) { r += "azimuth=" + azimuth.ToString() + " "; }
                    if (altitude != null) { r += "altitude=" + altitude.ToString() + " "; }
                    if (longitude != null) { r += "longitude=" + longitude.ToString() + " "; }
                    if (latitude != null) { r += "latitude=" + latitude.ToString() + " "; }
                    if (gnss_utc_time != null) { r += "gnss_utc_time=" + gnss_utc_time.ToString() + " "; }
                    if (mcc != null) { r += "mcc=" + mcc.ToString() + " "; }
                    if (mnc != null) { r += "mnc=" + mnc.ToString() + " "; }
                    if (lac != null) { r += "lac=" + lac.ToString() + " "; }
                    if (cell_id != null) { r += "cell_id=" + cell_id.ToString() + " "; }
                    if (position_append_mask != null) { r += "position_append_mask=" + position_append_mask.ToString() + " "; }
                    if (satellites_in_view != null) { r += "satellites_in_view=" + satellites_in_view.ToString() + " "; }
                    //...
                    if (total_distance != null) { r += "total_distance=" + total_distance.ToString() + " "; }
                    if (total_distance != null) { r += "total_distance_units=" + total_distance_units.ToString() + " "; }
                    if (total_fuel_used != null) { r += "total_fuel_used=" + total_fuel_used.ToString() + " "; }
                    if (engine_rpm != null) { r += "engine_rpm=" + engine_rpm.ToString() + " "; }
                    if (vehicle_speed != null) { r += "vehicle_speed=" + vehicle_speed.ToString() + " "; }
                    if (engine_coolant_temperature != null) { r += "engine_coolant_temperature=" + engine_coolant_temperature.ToString() + " "; }
                    if (fuel_consumption != null) { r += "fuel_consumption=" + fuel_consumption.ToString() + " "; }
                    if (fuel_level != null) { r += "fuel_level=" + fuel_level.ToString() + " "; }
                    if (fuel_level != null) { r += "fuel_level_units=" + fuel_level_units.ToString() + " "; }
                    if (range != null)                      { r += "range=" + range.ToString() + " "; }
                    if (range != null)                      { r += "range_units=" + range_units.ToString() + " "; }
                    if (accelerator_pedal_pressure != null) { r += "accelerator_pedal_pressure=" + accelerator_pedal_pressure.ToString() + " "; }
                    if (total_engine_hours != null) { r += "total_engine_hours=" + total_engine_hours.ToString() + " "; }
                    if (total_driving_time != null) { r += "total_driving_time=" + total_driving_time.ToString() + " "; }
                    if (total_engine_idle_time != null) { r += "total_engine_idle_time=" + total_engine_idle_time.ToString() + " "; }
                    if (total_idle_fuel_used != null) { r += "total_idle_fuel_used=" + total_idle_fuel_used.ToString() + " "; }
                    if (axle_weight_2nd != null)            { r += "axle_weight_2nd=" + axle_weight_2nd.ToString() + " "; }
                    if (detailed_information != null) { r += "detailed_information=" + detailed_information.ToString() + " "; }
                    if (lights != null)                     { r += "lights=" + lights.ToString() + " "; }
                    if (doors != null) { r += "doors=" + doors.ToString() + " "; }
                    if (send_time != null) { r += "send_time=" + send_time.ToString() + " "; }
                    if (count_number != null) { r += "count_number=" + count_number.ToString() + " "; }
                    if (tail_character != null) { r += "tail_character=" + tail_character.ToString() + " "; }
                    break;
            }

            return r;
        }

        public override bool? IsValid(object cadena)
        {
            string cadenaOriginal = cadena.ToString();
            return (cadenaOriginal.StartsWith("+RESP") || cadenaOriginal.StartsWith("+BUFF") ? true : false);
        }

        public override byte[] getAck(object cadena)
        {
            //Ejemplo: +SACK:DF0E$
            byte[] resultado = null;
            string[] Datos = ((string)cadena).Split(',');
            string ack = "+SACK:" + Datos[Datos.Length - 1];
            resultado = Encoding.ASCII.GetBytes(ack);
            return resultado;
        }


        public override string getCodigoGPS(object cadena)
        {
            codigoGPS = cadena.ToString().Split(',')[2];

            if ((codigoGPS != null) && (codigoGPS != "") && (Prefijo != null) && (Prefijo != ""))
            {
                codigoGPS = Prefijo + codigoGPS;
            }

            return codigoGPS;
        }

        public override byte[] getComando(object cadena, object aux)
        {
            return Encoding.ASCII.GetBytes(cadena.ToString());
        }

        public string HexToBinary(string hex, int cant_bytes)
        {
            // Normaliza la cadena a "cant_bytes" (2 x cant_bytes caracteres hex)
            hex = hex.PadLeft(2*cant_bytes, '0');

            // Convierte cada caracter hex a binario de 4 bits
            var binary = new StringBuilder();
            foreach (char c in hex)
            {
                // Convierte el caracter a su equivalente entero y luego a binario
                binary.Append(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'));
            }

            return binary.ToString();
        }

        public bool BitHabilitado(string binario, byte posicion_bit)
        {
            bool r = false;
            //Verifica el estado del bit (1=true, 0=false)
            //La posicion se considera "0" el bit menos significativo y "binario.Length-1" el mas significativo
            if ((posicion_bit >= 0) && (posicion_bit < binario.Length))
            {
                if (binario[binario.Length - 1 - posicion_bit] == '1')
                    r = true;
            }

            return r;
        }

        public override string getData(object cadenaOriginal)
        {
            string[] Datos = ((string)cadenaOriginal).Split(',');
            if (Datos.Length > 0)
            {
                codigoGPS = Datos[2];
                evento = Datos[0].Substring(6, Datos[0].Length - 6);
                numero_evento = (Eventos.ContainsKey(evento) ? Eventos[evento] : (int?)null);

                int i;
                string total_distance_str;
                string fuel_level_str;
                switch (evento)
                {
                    case "GTSTC":
                        velocidad = (Datos[6] != "" ? float.Parse(Datos[6]) : (float?)null);
                        heading = (Datos[7] != "" ? (short)float.Parse(Datos[7]) : (short?)null);
                        longitud = (Datos[9] != "" ? float.Parse(Datos[9]) : (float?)null);
                        latitud = (Datos[10] != "" ? float.Parse(Datos[10]) : (float?)null);
                        fechaGPS = (Datos[11] != "" ? DateTime.ParseExact(Datos[11], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal) : (DateTime?)null);
                        break;
                    case "GTBTC":
                    case "GTEPN":
                    case "GTEPF":
                        velocidad = (Datos[5] != "" ? float.Parse(Datos[5]) : (float?)null);
                        heading = (Datos[6] != "" ? (short)float.Parse(Datos[6]) : (short?)null);
                        longitud = (Datos[8] != "" ? float.Parse(Datos[8]) : (float?)null);
                        latitud = (Datos[9] != "" ? float.Parse(Datos[9]) : (float?)null);
                        fechaGPS = (Datos[10] != "" ? DateTime.ParseExact(Datos[10], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal) : (DateTime?)null);
                        break;
                    case "GTCAN":
                        /*
                         +RESP:GTCAN,270E01,863457050549706,Soflex,0,1,400A7DFF,,2,H1221530,895.81,1259,24,85,,P44.00,2,904.40,534.22,370.18,166.86,0680,00,,,0,29.8,313,35.1,-58.610707,-34.664033,20250618183702,20250618153704,3697$
                         */
                        i = 1; protocol_version = Datos[i];                                        //length=6  (XX0000 - XXFFFF, donde XX es 'A'-'Z','0'-'9' )
                        i += 1; unique_id = Datos[i];                                               //length=15 (IMEI)
                        i += 1; device_name = Datos[i];                                             //length<=20 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
                        i += 1; report_type = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);  //length=1 (0, 1, 2)
                        
                        i += 1; canbus_device_state = (Datos[i] != "" ? int.Parse(Datos[i]) : 0);   //length=1 (0, 1)
                        i += 1; canbus_report_mask = Datos[i]; //length<=8 (0 - FFFFFFFF) 
                        canbus_report_mask_bin = HexToBinary(canbus_report_mask, 4); //Binario de 4 bytes

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.VIN))
                        {
                            i += 1; vin = Datos[i]; //length=17 ('0'-'9' 'A'-'Z' excepto 'I' 'O' 'Q') Reservado en dispositivos CAN100
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.IGNITION_KEY))
                        {
                            i += 1; ignition_key = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1, 2)
                        }

                        total_distance = null;
                        total_distance_units = "";
                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_DISTANCE))
                        {
                            i += 1;
                            total_distance_str = Datos[i].Trim(); //length<=12 H(0 - 99999999) / I(0.0 - 2174483647.9)

                            if (total_distance_str != "")
                            {
                                if (total_distance_str[0] == 'H') //Unidades: Hectometros
                                {
                                    total_distance_units = "H";
                                }
                                else
                                if (total_distance_str[0] == 'I') //Unidades: Impulso de Distancia (??)
                                {
                                    total_distance_units = "I";

                                }
                                total_distance_str = total_distance_str.Substring(1);
                                total_distance = (total_distance_str != "" ? float.Parse(total_distance_str, CultureInfo.InvariantCulture) : 0);
                            }
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_FUEL_USED))
                        {
                            i += 1; total_fuel_used = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=9 (0.00 - 999999.99) Litros
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.ENGINE_RPM))
                        {
                            i += 1; engine_rpm = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 16383) RPM
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.VEHICLE_SPEED))
                        {
                            i += 1; vehicle_speed = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=3 (0 - 455) Km/h
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.ENGINE_COOLANT_TEMPERATURE))
                        {
                            i += 1; engine_coolant_temperature = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=4 (-40 - +215) ºC
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.FUEL_CONSUMPTION))
                        {
                            i += 1; fuel_consumption = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0.0 - 999.9) Litros cada 100 km
                        }

                        fuel_level = null;
                        fuel_level_units = "";
                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.FUEL_LEVEL))
                        {
                            i += 1; fuel_level_str = Datos[i].Trim(); //length<=7 L(0.00 - 9999.99) / P(0.00 - 100.00)

                            if (fuel_level_str != "")
                            {
                                if (fuel_level_str[0] == 'L') //Unidades: Litros
                                {
                                    fuel_level_units = "L";
                                }
                                else
                                if (fuel_level_str[0] == 'P') //Unidades: Porcentaje
                                {
                                    fuel_level_units = "P";
                                }
                                fuel_level_str = fuel_level_str.Substring(1);
                                fuel_level = (fuel_level_str != "" ? float.Parse(fuel_level_str, CultureInfo.InvariantCulture) : (float?)null);
                            }
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.RANGE))
                        {
                            i += 1; range = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0 - 99999999)
                            range_units = "H";
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.ACCELERATOR_PEDAL_PRESSURE))
                        {
                            i += 1; accelerator_pedal_pressure = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=3 (0 - 100) %
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_ENGINE_HOURS))
                        {
                            i += 1; total_engine_hours = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_DRIVING_TIME))
                        {
                            i += 1; total_driving_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_ENGINE_IDLE_TIME))
                        {
                            i += 1; total_engine_idle_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_IDLE_FUEL_USED))
                        {
                            i += 1; total_idle_fuel_used = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=9 (0.00 - 99999.99) Litros 
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.AXLE_WEIGHT_2ND))
                        {
                            i += 1; axle_weight_2nd = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 65535) Kg
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TACHOGRAPH_INFORMATION))
                        {
                            i += 1; tachograph_information = Datos[i]; //length=4 (0000 - FFFF)
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.DETAILED_INFORMATION))
                        {
                            i += 1; detailed_information = Datos[i]; //length=4 (0000 - FFFF)
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.LIGHTS))
                        {
                            i += 1; lights = Datos[i]; //length=2 (00 - FF)
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.DOORS))
                        {
                            i += 1; doors = Datos[i]; //length=2 (00 - FF)
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_VEHICLE_OVERSPEED_TIME))
                        {
                            i += 1; total_vehicle_overspeed_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_VEHICLE_ENGINE_OVERSPEED_TIME))
                        {
                            i += 1; total_vehicle_engine_overspeed_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.CAN_REPORT_EXPANSION_MASK))
                        { 
                            i += 1; can_expand_report_mask = Datos[i]; //length=8 (00000000 - FFFFFFFF)
                            can_expand_report_mask_bin = HexToBinary(can_expand_report_mask, 4); //Binario de 4 bytes
                            
                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AD_BLUE_LEVEL))
                            {
                                i += 1; ad_blue_level = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<3 (0 - 100) Litros
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AXLE_WEIGTH_1ST))
                            {
                                i += 1; axle_weigth_1st = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0 - 65535) Kg
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AXLE_WEIGTH_3RD))
                            {
                                i += 1; axle_weigth_3rd = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0 - 65535) Kg
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AXLE_WEIGTH_4TH))
                            {
                                i += 1; axle_weigth_4th = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0 - 65535) Kg
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAHP_OVERSPEED_SIGNAL))
                            {
                                i += 1; tachograhp_overspeed_signal = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1)
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAHP_VEHICLE_MOTION_SIGNAL))
                            {
                                i += 1; tachograhp_vehicle_motion_signal = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1)
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAHP_DRIVING_DIRECTION))
                            {
                                i += 1; tachograhp_driving_direction = (Datos[35] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1)
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.ANALOG_INPUT_VALUE))
                            {
                                i += 1; analog_input_value = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 999999) mV
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.ENGINE_BRAKING_FACTOR))
                            {
                                i += 1; engine_braking_factor = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=6 (0 - 999999)
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.PEDAL_BRAKING_FACTOR))
                            {
                                i += 1; pedal_braking_factor = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=6 (0 - 999999)    
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_ACCELERATOR_KICK_DOWNS))
                            {
                                i += 1; total_accelerator_kick_downs = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=6 (0 - 999999)
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_EFFECTIVE_ENGINE_SPEED_TIME))
                            {
                                i += 1; total_effective_engine_speed_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=10 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_CRUISE_CONTROL_TIME))
                            {
                                i += 1; total_cruise_control_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=10 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_ACCELERATOR_KICK_DOWN_TIME))
                            {
                                i += 1; total_accelerator_kick_down_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=10 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_BRAKES_APPLICATIONS))
                            {
                                i += 1; total_brakes_applications = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=6 (0 - 999999) 
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.SERVICE_DISTANCE))
                            {
                                i += 1; service_distance = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 65000) 
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_1_CARD_NUMBER))
                            {
                                i += 1; tachograph_driver_1_card_number = Datos[i]; //length<=10 (0 - 9999999999) 
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_2_CARD_NUMBER))
                            {
                                i += 1; tachograph_driver_2_card_number = Datos[i]; //length<=10 (0 - 9999999999) 
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_1_NAME))
                            {
                                i += 1; tachograph_driver_1_name = Datos[i]; //length<=40 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_2_NAME))
                            {
                                i += 1; tachograph_driver_2_name = Datos[i]; //length<=40 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.REGISTRATION_NUMBER))
                            {
                                i += 1; registration_number = Datos[i]; //length<=10 (0 - 9999999999) 
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.EXPANDED_INFORMATION))
                            {
                                i += 1; expanded_information = Datos[i]; //length=4 (0000 - FFFF)
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.RAPID_BRAKINGS))
                            {
                                i += 1; rapid_brakings = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=6 (0 - 999999) 
                            }

                            if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.RAPID_ACCELERATIONS))
                            {
                                i += 1; rapid_accelerations = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=6 (0 - 999999)                             
                            }
                        }                        

                        i += 1; reserved1 = Datos[i]; //reservado 1
                        i += 1; reserved2 = Datos[i]; //reservado 2 

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.GNSS_INFORMATION))
                        {                              
                            i += 1; gnss_accuracy = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                            //length<=2 (0) 
                            i += 1; gnss_speed = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);  //length<=5 (0.0 - 999.9) Km/h
                            i += 1; azimuth = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                                  //length<=3 (0 - 359) 
                            i += 1; altitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (-xxxxx.x) Metros
                            i += 1; longitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=11 (-180.0 - +180.0) 
                            i += 1; latitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);  //length<=10 (-90.0 - +90.0) 
                            i += 1; gnss_utc_time = (Datos[i] != "" ? DateTime.ParseExact(Datos[i], "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : (DateTime?)null); //length=14 (yyyyMMddHHmmss)
                        }

                        if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.CELL_INFORMATION))
                        {
                            i += 1; mcc = Datos[i];     //length=4 (0XXX) 
                            i += 1; mnc = Datos[i];     //length=4 (0XXX) 
                            i += 1; lac = Datos[i];     //length=4 (XXXX) 
                            i += 1; cell_id = Datos[i]; //length=4-8 (XXXX)
                        }
                        
                        if ((canbus_device_state != null) && (canbus_device_state == 1) &&
                           (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.GNSS_INFORMATION)))
                        {
                            i += 1; position_append_mask = Datos[i]; //length=2 (00, 01)
                        }

                        //i += 1; satellites_in_view = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1-2 (0 - 15)
                        i += 1; send_time = (Datos[i] != "" ? DateTime.ParseExact(Datos[i], "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : (DateTime?)null); //length=14 (yyyyMMddHHmmss)
                        i += 1; count_number = Datos[i].Substring(0, 4); //length=4 (0000 - FFFF) 
                        tail_character = Datos[i].Substring(4, 1); //length=1 ($) 

                        codigoGPS = unique_id;
                        velocidad = vehicle_speed;
                        longitud = longitude;
                        latitud = latitude;
                        if (gnss_utc_time != null)
                        {
                            fechaUtc = DateTime.SpecifyKind((DateTime)gnss_utc_time, DateTimeKind.Utc);
                            fechaGPS = fechaUtc?.ToLocalTime();
                        }                        
                        break;
                    case "GTERI":
                        /*
                         +RESP:GTERI,270E01,863457050549706,Soflex,00000004,,51,1,1,15.7,137,41.1,-58.611682,-34.664116,20250618183732,,,,,,9880.0,00918:57:23,,,100,220100,5,1,A7DFF,,2,H1221530,895.81,1059,13,83,,P44.00,4,904.41,534.23,370.18,166.86,06A0,00,20250618153733,369D$
                        */
                        i = 1; protocol_version = Datos[i];                                         //length=6  (XX0000 - XXFFFF, donde XX es 'A'-'Z','0'-'9' )
                        i += 1; unique_id = Datos[i];                                               //length=15 (IMEI)
                        i += 1; device_name = Datos[i];                                             //length<=20 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
                        i += 1; eri_mask = Datos[i];                                                //length=8 (0 - FFFFFFFF)
                        eri_mask_bin = HexToBinary(eri_mask, 4); //Binario de 4 bytes
                        
                        i += 1; external_power_supply = (Datos[i] != "" ? int.Parse(Datos[i]) : 0); //length<=5 (0 - 32000) mV
                        i += 1; report_id = (Datos[i] != "" ? int.Parse(Datos[i]) : 0);             //length=2 X(1-5) X(0-6)
                        i += 1; number = (Datos[i] != "" ? int.Parse(Datos[i]) : 0);                //length<=2 (1 - 15) 
                        
                        i += 1; gnss_accuracy = (Datos[i] != "" ? int.Parse(Datos[i]) : 0);         //length<=2 (0|1 - 50) 
                        i += 1; gnss_speed = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0.0 - 999.9) Km/h
                        i += 1; azimuth = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);      //length<=3 (0 - 359) 
                        i += 1; altitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);     //length<=8 (-xxxxx.x) Metros
                        i += 1; longitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=11 (-180.0 - +180.0) 
                        i += 1; latitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);  //length<=10 (-90.0 - +90.0) 
                        i += 1; gnss_utc_time = (Datos[i] != "" ? DateTime.ParseExact(Datos[i], "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : (DateTime?)null); //length=14 (yyyyMMddHHmmss)
                        i += 1; mcc = Datos[i];                                                                                 //length=4 (0XXX) 
                        i += 1; mnc = Datos[i];                                                                                 //length=4 (0XXX) 
                        i += 1; lac = Datos[i];                                                                                 //length=4 (XXXX) 
                        i += 1; cell_id = Datos[i];                                                                             //length=4-8 (XXXX)
                        i += 1; position_append_mask = Datos[i];                                                                //length=2 (00, 01)
                        //i += 1; satellites_in_view = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                     //length=1-2 (0 - 15)                                                                                                        //i += 1; satellites_in_view = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                     //length=1-2 (0 - 15)

                        i += 1; //mileage //length<=9 (0.0 - 4294967.0 Km)
                        i += 1; //hour_meter_count //length=11 (HHHHH:MM:SS)
                        i += 1; //analog_input_1 //length<=5 (0 - 16000 mV | F0 - F100)
                        i += 1; //analog_input_2 //length<=5 (0 - 16000 mV | F0 - F100)
                        i += 1; //backup_battery_percentage//length<=3 (0 - 100)
                        i += 1; //device_status //length<=10 (0000000000 - 0F0FFFFFFF)
                        i += 1; //uart_device_type //length<=2 (0 - 99)

                        if (BitHabilitado(eri_mask_bin, eri_mask_bits.DIGITAL_FUEL_SENSOR_DATA)) 
                        {
                            i += 1; //digital_fuel_sensor_data = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);  //length<=20 
                        }

                        if (BitHabilitado(eri_mask_bin, eri_mask_bits._1_WIRE_DATA))
                        {
                            i += 1; //_1_wire_device_number //length<=2 (0 - 19)
                            i += 1; //_1_wire_device_id //length=16
                            i += 1; //_1_wire_device_type //length=2
                            i += 1; //_1_wire_device_data //length<=40
                        }

                        if (BitHabilitado(eri_mask_bin, eri_mask_bits.CAN_DATA))
                        {
                            i += 1; canbus_device_state = (Datos[i] != "" ? int.Parse(Datos[i]) : 0);   //length=1 (0, 1)
                            i += 1; canbus_report_mask = Datos[i]; //length<=8 (0 - FFFFFFFF) 
                            canbus_report_mask_bin = HexToBinary(canbus_report_mask, 4); //Binario de 4 bytes

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.VIN))
                            {
                                i += 1; vin = Datos[i]; //length=17 ('0'-'9' 'A'-'Z' excepto 'I' 'O' 'Q') Reservado en dispositivos CAN100
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.IGNITION_KEY))
                            {
                                i += 1; ignition_key = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1, 2)
                            }

                            total_distance = null;
                            total_distance_units = "";
                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_DISTANCE))
                            {
                                i += 1;
                                total_distance_str = Datos[i].Trim(); //length<=12 H(0 - 99999999) / I(0.0 - 2174483647.9)

                                if (total_distance_str != "")
                                {
                                    if (total_distance_str[0] == 'H') //Unidades: Hectometros
                                    {
                                        total_distance_units = "H";
                                    }
                                    else
                                    if (total_distance_str[0] == 'I') //Unidades: Impulso de Distancia (??)
                                    {
                                        total_distance_units = "I";

                                    }
                                    total_distance_str = total_distance_str.Substring(1);
                                    total_distance = (total_distance_str != "" ? float.Parse(total_distance_str, CultureInfo.InvariantCulture) : 0);
                                }
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_FUEL_USED))
                            {
                                i += 1; total_fuel_used = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=9 (0.00 - 999999.99) Litros
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.ENGINE_RPM))
                            {
                                i += 1; engine_rpm = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 16383) RPM
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.VEHICLE_SPEED))
                            {
                                i += 1; vehicle_speed = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=3 (0 - 455) Km/h
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.ENGINE_COOLANT_TEMPERATURE))
                            {
                                i += 1; engine_coolant_temperature = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=4 (-40 - +215) ºC
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.FUEL_CONSUMPTION))
                            {
                                i += 1; fuel_consumption = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0.0 - 999.9) Litros cada 100 km
                            }

                            fuel_level = null;
                            fuel_level_units = "";
                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.FUEL_LEVEL))
                            {
                                i += 1; fuel_level_str = Datos[i].Trim(); //length<=7 L(0.00 - 9999.99) / P(0.00 - 100.00)

                                if (fuel_level_str != "")
                                {
                                    if (fuel_level_str[0] == 'L') //Unidades: Litros
                                    {
                                        fuel_level_units = "L";
                                    }
                                    else
                                    if (fuel_level_str[0] == 'P') //Unidades: Porcentaje
                                    {
                                        fuel_level_units = "P";
                                    }
                                    fuel_level_str = fuel_level_str.Substring(1);
                                    fuel_level = (fuel_level_str != "" ? float.Parse(fuel_level_str, CultureInfo.InvariantCulture) : (float?)null);
                                }
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.RANGE))
                            {
                                i += 1; range = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0 - 99999999)
                                range_units = "H";
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.ACCELERATOR_PEDAL_PRESSURE))
                            {
                                i += 1; accelerator_pedal_pressure = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=3 (0 - 100) %
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_ENGINE_HOURS))
                            {
                                i += 1; total_engine_hours = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_DRIVING_TIME))
                            {
                                i += 1; total_driving_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_ENGINE_IDLE_TIME))
                            {
                                i += 1; total_engine_idle_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_IDLE_FUEL_USED))
                            {
                                i += 1; total_idle_fuel_used = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=9 (0.00 - 99999.99) Litros 
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.AXLE_WEIGHT_2ND))
                            {
                                i += 1; axle_weight_2nd = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 65535) Kg
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TACHOGRAPH_INFORMATION))
                            {
                                i += 1; tachograph_information = Datos[i]; //length=4 (0000 - FFFF)
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.DETAILED_INFORMATION))
                            {
                                i += 1; detailed_information = Datos[i]; //length=4 (0000 - FFFF)
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.LIGHTS))
                            {
                                i += 1; lights = Datos[i]; //length=2 (00 - FF)
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.DOORS))
                            {
                                i += 1; doors = Datos[i]; //length=2 (00 - FF)
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_VEHICLE_OVERSPEED_TIME))
                            {
                                i += 1; total_vehicle_overspeed_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.TOTAL_VEHICLE_ENGINE_OVERSPEED_TIME))
                            {
                                i += 1; total_vehicle_engine_overspeed_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.CAN_REPORT_EXPANSION_MASK))
                            { 
                                i += 1; can_expand_report_mask = Datos[i]; //length=8 (00000000 - FFFFFFFF)
                                can_expand_report_mask_bin = HexToBinary(can_expand_report_mask, 4); //Binario de 4 bytes

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AD_BLUE_LEVEL))
                                {
                                    i += 1; ad_blue_level = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<3 (0 - 100) Litros
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AXLE_WEIGTH_1ST))
                                {
                                    i += 1; axle_weigth_1st = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0 - 65535) Kg
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AXLE_WEIGTH_3RD))
                                {
                                    i += 1; axle_weigth_3rd = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0 - 65535) Kg
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.AXLE_WEIGTH_4TH))
                                {
                                    i += 1; axle_weigth_4th = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=5 (0 - 65535) Kg
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAHP_OVERSPEED_SIGNAL))
                                {
                                    i += 1; tachograhp_overspeed_signal = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1)
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAHP_VEHICLE_MOTION_SIGNAL))
                                {
                                    i += 1; tachograhp_vehicle_motion_signal = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1)
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAHP_DRIVING_DIRECTION))
                                {
                                    i += 1; tachograhp_driving_direction = (Datos[35] != "" ? int.Parse(Datos[i]) : (int?)null); //length=1 (0, 1)
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.ANALOG_INPUT_VALUE))
                                {
                                    i += 1; analog_input_value = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 999999) mV
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.ENGINE_BRAKING_FACTOR))
                                {
                                    i += 1; engine_braking_factor = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=6 (0 - 999999)
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.PEDAL_BRAKING_FACTOR))
                                {
                                    i += 1; pedal_braking_factor = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=6 (0 - 999999)    
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_ACCELERATOR_KICK_DOWNS))
                                {
                                    i += 1; total_accelerator_kick_downs = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=6 (0 - 999999)
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_EFFECTIVE_ENGINE_SPEED_TIME))
                                {
                                    i += 1; total_effective_engine_speed_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=10 (0.00 - 99999.99) Horas
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_CRUISE_CONTROL_TIME))
                                {
                                    i += 1; total_cruise_control_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=10 (0.00 - 99999.99) Horas
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_ACCELERATOR_KICK_DOWN_TIME))
                                {
                                    i += 1; total_accelerator_kick_down_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=10 (0.00 - 99999.99) Horas
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TOTAL_BRAKES_APPLICATIONS))
                                {
                                    i += 1; total_brakes_applications = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=6 (0 - 999999) 
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.SERVICE_DISTANCE))
                                {
                                    i += 1; service_distance = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=5 (0 - 65000) 
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_1_CARD_NUMBER))
                                {
                                    i += 1; tachograph_driver_1_card_number = Datos[i]; //length<=10 (0 - 9999999999) 
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_2_CARD_NUMBER))
                                {
                                    i += 1; tachograph_driver_2_card_number = Datos[i]; //length<=10 (0 - 9999999999) 
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_1_NAME))
                                {
                                    i += 1; tachograph_driver_1_name = Datos[i]; //length<=40 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.TACHOGRAPH_DRIVER_2_NAME))
                                {
                                    i += 1; tachograph_driver_2_name = Datos[i]; //length<=40 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.REGISTRATION_NUMBER))
                                {
                                    i += 1; registration_number = Datos[i]; //length<=10 (0 - 9999999999) 
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.EXPANDED_INFORMATION))
                                {
                                    i += 1; expanded_information = Datos[i]; //length=4 (0000 - FFFF)
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.RAPID_BRAKINGS))
                                {
                                    i += 1; rapid_brakings = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=6 (0 - 999999) 
                                }

                                if (BitHabilitado(can_expand_report_mask_bin, can_expand_report_mask_bits.RAPID_ACCELERATIONS))
                                {
                                    i += 1; rapid_accelerations = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null); //length<=6 (0 - 999999)                             
                                }
                            }                            

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.GNSS_INFORMATION))
                            {
                                i += 1; gnss_accuracy = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                                 //length<=2 (0) 
                                i += 1; gnss_speed = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);  //length<=5 (0.0 - 999.9) Km/h
                                i += 1; azimuth = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                                       //length<=3 (0 - 359) 
                                i += 1; altitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);    //length<=8 (-xxxxx.x) Metros
                                i += 1; longitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);   //length<=11 (-180.0 - +180.0) 
                                i += 1; latitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);    //length<=10 (-90.0 - +90.0) 
                                i += 1; gnss_utc_time = (Datos[i] != "" ? DateTime.ParseExact(Datos[i], "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : (DateTime?)null); //length=14 (yyyyMMddHHmmss)
                            }

                            if (BitHabilitado(canbus_report_mask_bin, can_report_mask_bits.CELL_INFORMATION))
                            {
                                i += 1; mcc = Datos[i];     //length=4 (0XXX) 
                                i += 1; mnc = Datos[i];     //length=4 (0XXX) 
                                i += 1; lac = Datos[i];     //length=4 (XXXX) 
                                i += 1; cell_id = Datos[i]; //length=4-8 (XXXX)
                            }

                            /*if ((canbus_device_state != null) && (canbus_device_state == 1))
                            {
                                i += 1; position_append_mask = Datos[i]; //length=2 (00, 01)
                            }*/
                        }

                        if (BitHabilitado(eri_mask_bin, eri_mask_bits.FUEL_SENSOR_DATA_PERCENTAGE) ||
                            BitHabilitado(eri_mask_bin, eri_mask_bits.FUEL_SENSOR_DATA_VOLUME))
                        {
                            i += 1; //sensor_number //length<=3 (0 - 100)
                            i += 1; //sensor_type //length<=2 (0 - 8|20 - 21)
                            if (BitHabilitado(eri_mask_bin, eri_mask_bits.FUEL_SENSOR_DATA_PERCENTAGE))
                            {
                                i += 1; //percentage //length<=5 (0 - 100.0)
                            }
                            if (BitHabilitado(eri_mask_bin, eri_mask_bits.FUEL_SENSOR_DATA_VOLUME))
                            {
                                i += 1; //volume //length<=5 (0 - 10000.0)
                            }                            
                        }

                        if (BitHabilitado(eri_mask_bin, eri_mask_bits.RF433_ACCESORY_DATA))
                        {
                            i += 1; //accesory_number //length<=2 (0 - 10)
                            i += 1; //accesory_serial_number //length=5 (00001 - FFFFE)
                            i += 1; //accesory_type //length=1 (1|2)
                            i += 1; //accesory_temperature //length<=3 (-20 - 60)(x1ºC)
                            i += 1; //humidity //length<=3 (0 - 100)(x1%)
                        }

                        i += 1; send_time = (Datos[i] != "" ? DateTime.ParseExact(Datos[i], "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : (DateTime?)null); //length=14 (yyyyMMddHHmmss)
                        i += 1; count_number = Datos[i].Substring(0, 4); //length=4 (0000 - FFFF) 
                        tail_character = Datos[i].Substring(4, 1); //length=1 ($) 

                        codigoGPS = unique_id;
                        velocidad = vehicle_speed;
                        longitud = longitude;
                        latitud = latitude;
                        if (gnss_utc_time != null)
                        {
                            fechaUtc = DateTime.SpecifyKind((DateTime)gnss_utc_time, DateTimeKind.Utc);
                            fechaGPS = fechaUtc?.ToLocalTime();
                        }
                        break;
                    case "GTFRI":
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTIGN": //Ingnition on
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTIGF": //Ingnition off
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTSTT": //Status motion
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTHBM": //Hash behavior
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTDOG": 
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTPDP":
                        fechaGPS = DateTime.UtcNow;
                        break;                        
                    case "GTPNA":
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTPFA":
                        fechaGPS = DateTime.UtcNow;
                        break;
                    case "GTSOS": //SOS
                        HDOP = (Datos[7] != "" ? float.Parse(Datos[7]) : (float?)null);
                        velocidad = (Datos[8] != "" ? float.Parse(Datos[8]) : (float?)null);
                        heading = (Datos[9] != "" ? (short)float.Parse(Datos[9]) : (short?)null);
                        longitud = (Datos[11] != "" ? float.Parse(Datos[11]) : (float?)null);
                        latitud = (Datos[12] != "" ? float.Parse(Datos[12]) : (float?)null);
                        //fechaGPS = (Datos[13] != "" ? DateTime.ParseExact(Datos[13], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal) : (DateTime?)null);
                        fechaGPS = DateTime.UtcNow;
                        Bateria = (Datos[19] != "" ? float.Parse(Datos[19]) : (float?)null);
                        break;
                    default:
                        HDOP = (Datos[7] != "" ? float.Parse(Datos[7]) : (float?)null);
                        velocidad = (Datos[8] != "" ? float.Parse(Datos[8]) : (float?)null);
                        heading = (Datos[9] != "" ? (short)float.Parse(Datos[9]) : (short?)null);
                        longitud = (Datos[11] != "" ? float.Parse(Datos[11]) : (float?)null);
                        latitud = (Datos[12] != "" ? float.Parse(Datos[12]) : (float?)null);
                        fechaGPS = (Datos[13] != "" ? DateTime.ParseExact(Datos[13], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal) : (DateTime?)null);
                        Bateria = (Datos[19] != "" ? float.Parse(Datos[19]) : (float?)null);
                        break;
                }

                XGatewayBase.XGateway.EnviarPosicion(
                  CodigoGPS: "GL-" + codigoGPS,
                  HDOP: HDOP,
                  Velocidad: velocidad,
                  Heading: heading,
                  Longitud: (longitud == 0 ? (float?)null : longitud),
                  Latitud: (latitud == 0 ? (float?)null : latitud),
                  FechaGPS: fechaGPS,
                  Bateria: Bateria,
                  Evento: numero_evento,
                  TramaOriginal: cadenaOriginal.ToString()
              );

                return cadenaOriginal.ToString();
            }
            else
                return "";
        }

        public override List<string> GetGpsJsonData()
        {
            //para las tramas cabeceras que no se parsean, no retornar JsonData --------------- 
            if ((latitud == null) || (longitud == null) || (latitud == 0) || (longitud == 0)) {
                return null;
            }

            float? total_distance_aux;
            total_distance_aux = (total_distance != null) ? total_distance / 10 : null; //En hectometros, dividir por 10 para pasar a Km

            float? external_power_supply_aux;
            external_power_supply_aux = (external_power_supply != null) ? external_power_supply / 1000 : null; //En mV, dividir por 1000 para pasar a V
            if ((external_power_supply_aux == null) && (Bateria != null))
            {
                external_power_supply_aux = Bateria;
            }

            int? ignition_key_aux = null;
            if (ignition_key != null )
            {
                ignition_key_aux = (ignition_key > 0) ? 1 : 0;
            }

            GpsJsonData GpsJsonData = new GpsJsonData();
            GpsJsonData.SetData(
                //paquete ---
                trama: null,

                //movil --
                id_equipo_avle:  null,
                id_movil_avle:  null,
                id_equipo_mapsoft: null,
                id_movil_mapsoft: null,

                //gps ---
                id_gps: codigoGPS,
                id_gps_prefijo: Prefijo,
                fecha_gps_utc: fechaUtc,
                fecha_gps_local: fechaGPS,
                fecha_server_local: DateTime.Now,
                lat: latitud,
                lon: longitud,
                alt: altitude,
                rumbo: heading,
                id_evento: null,
                vel_gps: gnss_speed,
                sat: null, //satellites_in_view
                acc: gnss_accuracy,
                azim: azimuth,
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
                vel: vehicle_speed,
                bat: external_power_supply_aux,
                ign: ignition_key_aux,
                odo: total_distance_aux,
                comb_total: null,
                comb_total_p: null,
                rpm: engine_rpm,
                temp: engine_coolant_temperature,
                consumo_comb: fuel_consumption, 
                comb: (fuel_level_units == "L") ? fuel_level : null,
                comb_p: (fuel_level_units == "P") ? fuel_level : null,
                acel: accelerator_pedal_pressure,   
                tmotor_func: total_engine_hours,
                tmotor_func_vel: total_driving_time,
                tmotor_func_vel0: total_engine_idle_time,
                comb_vel0: total_idle_fuel_used,
                pres_aceite: null
                );
            
            List<string> jsonList = new List<string>();
            jsonList.Add(GpsJsonData.GetDataJson());
            return jsonList;
        }
    }
}
