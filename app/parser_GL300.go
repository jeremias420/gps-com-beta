package app

/*
import (
	"fmt"
	"strconv"
	"strings"
	"time"
)

const (
	//can_report_mask_bits
	CANMASK_CELL_INFORMATION                      	byte = 31
	CANMASK_GNSS_INFORMATION                      	byte = 30
	CANMASK_CAN_REPORT_EXPANSION_MASK             	byte = 29
	CANMASK_RESERVED_28                           	byte = 28
	CANMASK_RESERVED_27                           	byte = 27
	CANMASK_RESERVED_26                           	byte = 26
	CANMASK_RESERVED_25                           	byte = 25
	CANMASK_RESERVED_24                           	byte = 24
	CANMASK_RESERVED_23                           	byte = 23
	CANMASK_TOTAL_DISTANCE_IMPULSES               	byte = 22
	CANMASK_TOTAL_VEHICLE_ENGINE_OVERSPEED_TIME   	byte = 21
	CANMASK_TOTAL_VEHICLE_OVERSPEED_TIME          	byte = 20
	CANMASK_DOORS                                 	byte = 19
	CANMASK_LIGHTS                                	byte = 18
	CANMASK_DETAILED_INFORMATION                  	byte = 17
	CANMASK_TACHOGRAPH_INFORMATION                	byte = 16
	CANMASK_AXLE_WEIGHT_2ND                       	byte = 15
	CANMASK_TOTAL_IDLE_FUEL_USED                  	byte = 14
	CANMASK_TOTAL_ENGINE_IDLE_TIME                	byte = 13
	CANMASK_TOTAL_DRIVING_TIME                    	byte = 12
	CANMASK_TOTAL_ENGINE_HOURS                    	byte = 11
	CANMASK_ACCELERATOR_PEDAL_PRESSURE            	byte = 10
	CANMASK_RANGE                                 	byte = 9
	CANMASK_FUEL_LEVEL                            	byte = 8
	CANMASK_FUEL_CONSUMPTION                      	byte = 7
	CANMASK_ENGINE_COOLANT_TEMPERATURE            	byte = 6
	CANMASK_ENGINE_RPM                            	byte = 5
	CANMASK_VEHICLE_SPEED                         	byte = 4
	CANMASK_TOTAL_FUEL_USED                       	byte = 3
	CANMASK_TOTAL_DISTANCE                        	byte = 2
	CANMASK_IGNITION_KEY                          	byte = 1
	CANMASK_VIN                                   	byte = 0

	//can_expand_report_mask_bits
	CANMASKEXPAND_RESERVED_31                     	byte = 31;
	CANMASKEXPAND_RESERVED_30                     	byte = 30;
	CANMASKEXPAND_RESERVED_29                     	byte = 29;
	CANMASKEXPAND_RESERVED_28                     	byte = 28;
	CANMASKEXPAND_RESERVED_27                     	byte = 27;
	CANMASKEXPAND_RESERVED_26                     	byte = 26;
	CANMASKEXPAND_RESERVED_25                     	byte = 25;
	CANMASKEXPAND_SERVICE_DISTANCE                	byte = 24;
	CANMASKEXPAND_RESERVED_23                     	byte = 23;
	CANMASKEXPAND_RAPID_ACCELERATIONS             	byte = 22;
	CANMASKEXPAND_RAPID_BRAKINGS                  	byte = 21;
	CANMASKEXPAND_EXPANDED_INFORMATION            	byte = 20;
	CANMASKEXPAND_REGISTRATION_NUMBER 			  	byte = 19;
	CANMASKEXPAND_TACHOGRAPH_DRIVER_2_NAME 		  	byte = 18;
	CANMASKEXPAND_TACHOGRAPH_DRIVER_1_NAME        	byte = 17;
	CANMASKEXPAND_TACHOGRAPH_DRIVER_2_CARD_NUMBER 	byte = 16;
	CANMASKEXPAND_TACHOGRAPH_DRIVER_1_CARD_NUMBER 	byte = 15;
	CANMASKEXPAND_TOTAL_BRAKES_APPLICATIONS       	byte = 14;
	CANMASKEXPAND_TOTAL_ACCELERATOR_KICK_DOWN_TIME 	byte = 13;
	CANMASKEXPAND_TOTAL_CRUISE_CONTROL_TIME        	byte = 12;
	CANMASKEXPAND_TOTAL_EFFECTIVE_ENGINE_SPEED_TIME byte = 11;
	CANMASKEXPAND_TOTAL_ACCELERATOR_KICK_DOWNS     	byte = 10;
	CANMASKEXPAND_PEDAL_BRAKING_FACTOR             	byte = 9;
	CANMASKEXPAND_ENGINE_BRAKING_FACTOR            	byte = 8;
	CANMASKEXPAND_ANALOG_INPUT_VALUE               	byte = 7;
	CANMASKEXPAND_TACHOGRAHP_DRIVING_DIRECTION     	byte = 6;
	CANMASKEXPAND_TACHOGRAHP_VEHICLE_MOTION_SIGNAL 	byte = 5;
	CANMASKEXPAND_TACHOGRAHP_OVERSPEED_SIGNAL      	byte  = 4;
	CANMASKEXPAND_AXLE_WEIGTH_4TH                  	byte = 3;
	CANMASKEXPAND_AXLE_WEIGTH_3RD                 	byte = 2;
	CANMASKEXPAND_AXLE_WEIGTH_1ST                  	byte = 1;
	CANMASKEXPAND_AD_BLUE_LEVEL                    	byte = 0;

	//eri_mask_bits
	ERIMASK_RF433_ACCESORY_DATA 					byte = 7;
	ERIMASK_RESERVED_6 								byte = 6;
	ERIMASK_RESERVED_5 								byte = 5;
	ERIMASK_FUEL_SENSOR_DATA_VOLUME 				byte = 4; //Bloque de datos: Fuel Sensor Data (Volumen)
	ERIMASK_FUEL_SENSOR_DATA_PERCENTAGE 			byte = 3; //Bloque de datos: Fuel Sensor Data (Porcentaje)
	ERIMASK_CAN_DATA 								byte = 2;
	ERIMASK_1_WIRE_DATA 							byte = 1;
	ERIMASK_DIGITAL_FUEL_SENSOR_DATA 				byte = 0;
)

type QueclinkGV300W struct{
	GPSData

	// GTERI ------------------------------------
	ERIMask             string
	ExternalPowerSupply *int
	ReportID            *int
	Number              *int
}

func BitHabilitado(binario string, posicionBit byte) bool {
	// Verifica el estado del bit (1=true, 0=false)
	// La posición 0 es el bit menos significativo (el más a la derecha)
	if int(posicionBit) >= 0 && int(posicionBit) < len(binario) {
		if binario[len(binario)-1-int(posicionBit)] == '1' {
			return true
		}
	}
	return false
}


// HexToBinary convierte una cadena hexadecimal a binario, normalizando a cantBytes bytes
func HexToBinary(hex string, cantBytes int) string {
	// Normaliza la cadena a "cantBytes" (2 * cantBytes caracteres hex), con ceros a la izquierda
	hex = strings.Repeat("0", 2*cantBytes-len(hex)) + hex

	var binary strings.Builder
	for _, c := range hex {
		// Convierte el carácter a entero base 16
		n, err := strconv.ParseInt(string(c), 16, 64)
		if err != nil {
			continue // o podés manejar el error como prefieras
		}
		// Convierte a binario de 4 bits y lo agrega al resultado
		binary.WriteString(fmt.Sprintf("%04b", n))
	}
	return binary.String()
}

func parseInt(s string) *int {
	if s == "" {
		return nil
	}
	val, err := strconv.Atoi(s)
	if err != nil {
		return nil
	}
	return &val
}

func parseShort(s string) *int {
	return parseInt(s)
}

func parseDateTime(s string) *time.Time {
	if s == "" {
		return nil
	}
	t, err := time.ParseInLocation("20060102150405", s, time.UTC)
	if err != nil {
		return nil
	}
	return &t
}

func getEventCode(event string) *int {
	if code, ok := eventos[event]; ok {
		return &code
	}
	return nil
}

var eventos = map[string]int{
	"GTPNA": 1,
	"GTPFA": 2,
	"GTEPN": 3,
	"GTEPF": 4,
	"GTBPL": 5,
	"GTBTC": 6,
	"GTSTC": 7,
	"GTIGN": 8,
	"GTIGF": 9,
	"GTUPC": 10,
	"GTSOS": 11,
	"GTCAN": 12,
	"GTFRI": 13,
	"GTERI": 14,
	"GTSTT": 15,
	"GTHBM": 16,
}
func (g *QueclinkGV300W) GetGpsJsonData() string{

}
func (g *QueclinkGV300W) GetData() []byte{
	return nil
}
func (g *QueclinkGV300W) GetComando() string{
	return nil
}
func (g *QueclinkGV300W) GetCodigoGPS() string{
	return nil
}
func (g *QueclinkGV300W) GetAck() []byte{
	return nil
}
func (g *QueclinkGV300W) IsValid() bool{
	return nil
}
func (g *QueclinkGV300W) SendACK() bool{
	return nil
}

func inc(idx int) int {
    return idx + 1
}

func NewQueclinkGV300W(trama string) *QueclinkGV300W {
	datos := strings.Split(trama, ",")
	if len(datos) < 3 {
		return nil
	}
	evento := datos[0][6:]
	codigoGPS := datos[2]
	numeroEvento := eventos[evento]
	var gps GPSData
	gps.CodigoGPS = "GL-" + codigoGPS
	gps.Evento = &numeroEvento
	gps.Trama = trama
	switch evento {
	case "GTSTC":
		gps.Velocidad = parseFloat(datos[6])
		gps.Heading = parseInt(datos[7])
		gps.Longitud = parseFloat(datos[9])
		gps.Latitud = parseFloat(datos[10])
		gps.FechaGPS = parseDate(datos[11])
	case "GTBTC", "GTEPN", "GTEPF":
		gps.Velocidad = parseFloat(datos[5])
		gps.Heading = parseInt(datos[6])
		gps.Longitud = parseFloat(datos[8])
		gps.Latitud = parseFloat(datos[9])
		gps.FechaGPS = parseDate(datos[10])
	case "GTCAN":

		// +RESP:GTCAN,270E01,863457050549706,Soflex,0,1,400A7DFF,,2,H1221530,895.81,1259,24,85,,P44.00,2,904.40,534.22,370.18,166.86,0680,00,,,0,29.8,313,35.1,-58.610707,-34.664033,20250618183702,20250618153704,3697$
		i := 0;
		gps.Protocol_version = datos[inc(i)];              	//length=6  (XX0000 - XXFFFF, donde XX es 'A'-'Z','0'-'9' )
		gps.Unique_id = datos[inc(i)];                     	//length=15 (IMEI)
		gps.Device_name = datos[inc(i)];                 	//length<=20 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
		gps.Report_type = parseInt(datos[inc(i)])			//length=1 (0, 1, 2)

		gps.Canbus_device_state = parseInt(datos[inc(i)])   //length=1 (0, 1)
        gps.Canbus_report_mask = datos[inc(i)]; //length<=8 (0 - FFFFFFFF)
        gps.Canbus_report_mask_bin = HexToBinary(canbus_report_mask, 4); //Binario de 4 bytes

		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_VIN)){
			gps.Vin = Datos[inc(i)]; //length=17 ('0'-'9' 'A'-'Z' excepto 'I' 'O' 'Q') Reservado en dispositivos CAN100
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_IGNITION_KEY)){
			gps.Ignition_key = parseInt(datos[inc(i)]) //length=1 (0, 1, 2)
		}

		gps.Total_distance = null
		gps.Total_distance_units = ""
		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_TOTAL_DISTANCE)){
			gps.Total_distance_str = strings.TrimSpace(datos[inc(i)]) //length<=12 H(0 - 99999999) / I(0.0 - 2174483647.9)

			if (gps.Total_distance_str != ""){
				if (gps.Total_distance_str[0] == 'H'){//Unidades: Hectometros
					gps.Total_distance_units = "H"
				}else if (gps.Total_distance_str[0] == 'I'){ //Unidades: Impulso de Distancia (??)
					gps.Total_distance_units = "I";
				}
				gps.Total_distance_str = gps.Total_distance_str.Substring(1);
				gps.Total_distance = parseFloat(gps.Total_distance_str)
			}
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_TOTAL_FUEL_USED)){
			gps.Total_fuel_used = parseFloat(datos[inc(i)]) //length<=9 (0.00 - 999999.99) Litros
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_ENGINE_RPM)){
			gps.Engine_rpm = parseInt(datos[inc(i)]) //length<=5 (0 - 16383) RPM
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_VEHICLE_SPEED)){
			gps.Vehicle_speed = parseFloat(datos[inc(i)]) //length<=3 (0 - 455) Km/h
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_ENGINE_COOLANT_TEMPERATURE)){
			gps.Engine_coolant_temperature = parseInt(datos[inc(i)])  //length<=4 (-40 - +215) ºC
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_FUEL_CONSUMPTION)){
			gps.Fuel_consumption = parseFloat(datos[inc(i)]) //length<=5 (0.0 - 999.9) Litros cada 100 km
		}

		gps.Fuel_level = null;
		gps.Fuel_level_units = "";
		if (BitHabilitado(gps.Canbus_report_mask_bin, CANMASK_FUEL_LEVEL)){
			gps.Fuel_level_str = strings.TrimSpace(datos[inc(i)]) //length<=7 L(0.00 - 9999.99) / P(0.00 - 100.00)

			if (gps.Fuel_level_str != ""){
				if (gps.Fuel_level_str[0] == 'L'){ //Unidades: Litros
					gps.Fuel_level_units = "L"
				} else if (gps.Fuel_level_str[0] == 'P'){ //Unidades: Porcentaje
					gps.Fuel_level_units = "P"
				}
				gps.Fuel_level_str = gps.Fuel_level_str[1:]
				gps.Fuel_level = parseFloat(gps.Fuel_level_str)
			}
		}
		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.RANGE)){
			gps.Range = parseFloat(datos[inc(i)]) //length<=8 (0 - 99999999)
			gps.Range_units = "H";
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.ACCELERATOR_PEDAL_PRESSURE)){
			gps.Accelerator_pedal_pressure = parseFloat(datos[inc(i)]) //length<=3 (0 - 100) %
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.TOTAL_ENGINE_HOURS)){
			gps.Total_engine_hours = parseFloat(datos[inc(i)]) //length<=8 (0.00 - 99999.99) Horas
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.TOTAL_DRIVING_TIME)){
			gps.Total_driving_time = parseFloat(datos[inc(i)]) //length<=8 (0.00 - 99999.99) Horas
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.TOTAL_ENGINE_IDLE_TIME)){
			gps.Total_engine_idle_time = parseFloat(datos[inc(i)]) //length<=8 (0.00 - 99999.99) Horas
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.TOTAL_IDLE_FUEL_USED)){
			gps.Total_idle_fuel_used = parseFloat(datos[inc(i)]) //length<=9 (0.00 - 99999.99) Litros
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.AXLE_WEIGHT_2ND)){
			gps.Axle_weight_2nd = parseInt(datos[inc(i)]) //length<=5 (0 - 65535) Kg
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.TACHOGRAPH_INFORMATION)){
			gps.Tachograph_information = Datos[inc(i)]; //length=4 (0000 - FFFF)
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.DETAILED_INFORMATION)){
			gps.Detailed_information = Datos[inc(i)]; //length=4 (0000 - FFFF)
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.LIGHTS)){
			gps.Lights = Datos[inc(i)]; //length=2 (00 - FF)
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.DOORS)){
			gps.Doors = Datos[inc(i)]; //length=2 (00 - FF)
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.TOTAL_VEHICLE_OVERSPEED_TIME)){
			gps.Total_vehicle_overspeed_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (0.00 - 99999.99) Horas
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.TOTAL_VEHICLE_ENGINE_OVERSPEED_TIME)){
			gps.Total_vehicle_engine_overspeed_time = parseFloat(Datos[inc(i)])//length<=8 (0.00 - 99999.99) Horas
		}

		if (BitHabilitado(gps.Canbus_report_mask_bin, can_report_mask_bits.CAN_REPORT_EXPANSION_MASK))
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









		gps.Canbus_device_state = parseInt(datos[inc(i)])  	  //length=1 (0, 1)
		gps.Canbus_report_mask = datos[inc(i)];               //length<=8 (0 - FFFFFFFF)
		gps.Vin = datos[inc(i)];                              //length=17 ('0'-'9' 'A'-'Z' excepto 'I' 'O' 'Q') Reservado en dispositivos CAN100
		gps.Ignition_key = parseInt(datos[inc(i)])			  //length=1 (0, 1, 2)

		gps.Total_distance_str = datos[inc(i)].Trim();        //length<=12 H(0 - 99999999) / I(0.0 - 2174483647.9)
		gps.Total_distance = null;
		gps.Total_distance_units = "";
		if (gps.Total_distance_str != ""){
			if (gpsTotal_distance_str[0] == "H") {//Unidades: Hectometros
				gps.Total_distance_units = "H";
			} else if (gps.Total_distance_str[0] == "I"){ //Unidades: Impulso de Distancia (??)
				gps.Total_distance_units = "I";
			}
			gps.Total_distance_str = gps.Total_distance_str.Substring(1)
			gps.Total_distance = parseFloat(gps.Total_distance_str)
		}

		gps.Total_fuel_used = parseFloat(datos[inc(i)])     //length<=9 (0.00 - 999999.99) Litros
		gps.Engine_rpm =  parseInt(datos[inc(i)])                                                //length<=5 (0 - 16383) RPM
		gps.Vehicle_speed = parseFloat(datos[inc(i)])            //length<=3 (0 - 455) Km/h
		gps.Engine_coolant_temperature = parseInt(datos[inc(i)])                               //length<=4 (-40 - +215) ºC
		gps.Fuel_consumption = parseFloat(datos[inc(i)])        //length<=5 (0.0 - 999.9) Litros cada 100 km

		gps.Fuel_level_str = Datos[inc(i)].Trim();                                                                        //length<=7 L(0.00 - 9999.99) / P(0.00 - 100.00)
		gps.Fuel_level = null;
		gps.Fuel_level_units = "";
		if (gps.Fuel_level_str != ""){
			if (fuel_level_str[0] == 'L'){ //Unidades: Litros
				fuel_level_units = "L";
			} else if (fuel_level_str[0] == 'P'){ //Unidades: Porcentaje
				fuel_level_units = "P";
			}
			fuel_level_str = fuel_level_str.Substring(1);
			fuel_level = (fuel_level_str != "" ? float.Parse(fuel_level_str, CultureInfo.InvariantCulture) : (float?)null);
		}


		i += 1; accelerator_pedal_pressure = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);             //length<=3 (0 - 100) %
		i += 1; total_engine_hours = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);                     //length<=8 (0.00 - 99999.99) Horas
		i += 1; total_driving_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);                     //length<=8 (0.00 - 99999.99) Horas
		i += 1; total_engine_idle_time = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);                 //length<=8 (0.00 - 99999.99) Horas
		i += 1; total_idle_fuel_used = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);                   //length<=9 (0.00 - 99999.99) Litros
		i += 1; detailed_information = Datos[i];                                                                                                //length=4 (0000 - FFFF)
		i += 1; doors = Datos[i];                                                                                                               //length=2 (00 - FF)
		i += 1; can_expand_report_mask = Datos[i];                                                                                              //length=8 (00000000 - FFFFFFFF)
		i += 1; reserved1 = Datos[i];                                                                           //reservado 1
		i += 1; gnss_accuracy = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                            //length<=2 (0)
		i += 1; gnss_speed = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);  //length<=5 (0.0 - 999.9) Km/h
		i += 1; azimuth = (Datos[i] != "" ? int.Parse(Datos[i]) : (int?)null);                                  //length<=3 (0 - 359)
		i += 1; altitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=8 (-xxxxx.x) Metros
		i += 1; longitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null); //length<=11 (-180.0 - +180.0)
		i += 1; latitude = (Datos[i] != "" ? float.Parse(Datos[i], CultureInfo.InvariantCulture) : (float?)null);  //length<=10 (-90.0 - +90.0)
		i += 1; gnss_utc_time = (Datos[i] != "" ? DateTime.ParseExact(Datos[i], "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : (DateTime?)null); //length=14 (yyyyMMddHHmmss)
		i += 1; send_time = (Datos[i] != "" ? DateTime.ParseExact(Datos[i], "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : (DateTime?)null); //length=14 (yyyyMMddHHmmss)
		i += 1; count_number = Datos[i].Substring(0, 4);                                                           //length=4 (0000 - FFFF)
		tail_character = Datos[i].Substring(4, 1);                                                                 //length=1 ($)

		codigoGPS = unique_id;
		velocidad = vehicle_speed;
		longitud = longitude;
		latitud = latitude;
		fechaUtc = DateTime.SpecifyKind((DateTime)gnss_utc_time, DateTimeKind.Utc);
		fechaGPS = fechaUtc?.ToLocalTime();
		break;



	case "GTERI":
		i := 19 // salto hasta gnss_speed
		gps.Velocidad = parseFloat(datos[i])
		i += 1
		gps.Heading = parseInt(datos[i])
		i += 1
		gps.Longitud = parseFloat(datos[i])
		i += 1
		gps.Latitud = parseFloat(datos[i])
		i += 1
		gps.FechaGPS = parseDate(datos[i])
	case "GTSOS":
		gps.HDOP = parseFloat(datos[7])
		gps.Velocidad = parseFloat(datos[8])
		gps.Heading = parseInt(datos[9])
		gps.Longitud = parseFloat(datos[11])
		gps.Latitud = parseFloat(datos[12])
		gps.FechaGPS = ptrTime(time.Now())
		gps.Bateria = parseFloat(datos[19])
	case "GTFRI", "GTIGN", "GTIGF", "GTSTT", "GTHBM":
		gps.FechaGPS = ptrTime(time.Now())
	default:
		gps.HDOP = parseFloat(datos[7])
		gps.Velocidad = parseFloat(datos[8])
		gps.Heading = parseInt(datos[9])
		gps.Longitud = parseFloat(datos[11])
		gps.Latitud = parseFloat(datos[12])
		gps.FechaGPS = parseDate(datos[13])
		gps.Bateria = parseFloat(datos[19])
	}

	// Enviar al bulk
	fmt.Printf("POS: %+v\n", gps)
	return &gps
}
*/
