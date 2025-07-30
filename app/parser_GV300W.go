package app

import (
	"fmt"
	"strconv"
	"strings"
	"time"
)

const (
	//can_report_mask_bits
	CANMASK_CELL_INFORMATION                    byte = 31
	CANMASK_GNSS_INFORMATION                    byte = 30
	CANMASK_CAN_REPORT_EXPANSION_MASK           byte = 29
	CANMASK_RESERVED_28                         byte = 28
	CANMASK_RESERVED_27                         byte = 27
	CANMASK_RESERVED_26                         byte = 26
	CANMASK_RESERVED_25                         byte = 25
	CANMASK_RESERVED_24                         byte = 24
	CANMASK_RESERVED_23                         byte = 23
	CANMASK_TOTAL_DISTANCE_IMPULSES             byte = 22 //no se encuentra
	CANMASK_TOTAL_VEHICLE_ENGINE_OVERSPEED_TIME byte = 21
	CANMASK_TOTAL_VEHICLE_OVERSPEED_TIME        byte = 20
	CANMASK_DOORS                               byte = 19
	CANMASK_LIGHTS                              byte = 18
	CANMASK_DETAILED_INFORMATION                byte = 17
	CANMASK_TACHOGRAPH_INFORMATION              byte = 16
	CANMASK_AXLE_WEIGHT_2ND                     byte = 15
	CANMASK_TOTAL_IDLE_FUEL_USED                byte = 14
	CANMASK_TOTAL_ENGINE_IDLE_TIME              byte = 13
	CANMASK_TOTAL_DRIVING_TIME                  byte = 12
	CANMASK_TOTAL_ENGINE_HOURS                  byte = 11
	CANMASK_ACCELERATOR_PEDAL_PRESSURE          byte = 10
	CANMASK_RANGE                               byte = 9
	CANMASK_FUEL_LEVEL                          byte = 8
	CANMASK_FUEL_CONSUMPTION                    byte = 7
	CANMASK_ENGINE_COOLANT_TEMPERATURE          byte = 6
	CANMASK_ENGINE_RPM                          byte = 5
	CANMASK_VEHICLE_SPEED                       byte = 4
	CANMASK_TOTAL_FUEL_USED                     byte = 3
	CANMASK_TOTAL_DISTANCE                      byte = 2
	CANMASK_IGNITION_KEY                        byte = 1
	CANMASK_VIN                                 byte = 0

	//can_expand_report_mask_bits
	CANMASKEXPAND_RESERVED_31                       byte = 31
	CANMASKEXPAND_RESERVED_30                       byte = 30
	CANMASKEXPAND_RESERVED_29                       byte = 29
	CANMASKEXPAND_RESERVED_28                       byte = 28
	CANMASKEXPAND_RESERVED_27                       byte = 27
	CANMASKEXPAND_RESERVED_26                       byte = 26
	CANMASKEXPAND_RESERVED_25                       byte = 25
	CANMASKEXPAND_SERVICE_DISTANCE                  byte = 24
	CANMASKEXPAND_RESERVED_23                       byte = 23
	CANMASKEXPAND_RAPID_ACCELERATIONS               byte = 22
	CANMASKEXPAND_RAPID_BRAKINGS                    byte = 21
	CANMASKEXPAND_EXPANDED_INFORMATION              byte = 20
	CANMASKEXPAND_REGISTRATION_NUMBER               byte = 19
	CANMASKEXPAND_TACHOGRAPH_DRIVER_2_NAME          byte = 18
	CANMASKEXPAND_TACHOGRAPH_DRIVER_1_NAME          byte = 17
	CANMASKEXPAND_TACHOGRAPH_DRIVER_2_CARD_NUMBER   byte = 16
	CANMASKEXPAND_TACHOGRAPH_DRIVER_1_CARD_NUMBER   byte = 15
	CANMASKEXPAND_TOTAL_BRAKES_APPLICATIONS         byte = 14
	CANMASKEXPAND_TOTAL_ACCELERATOR_KICK_DOWN_TIME  byte = 13
	CANMASKEXPAND_TOTAL_CRUISE_CONTROL_TIME         byte = 12
	CANMASKEXPAND_TOTAL_EFFECTIVE_ENGINE_SPEED_TIME byte = 11
	CANMASKEXPAND_TOTAL_ACCELERATOR_KICK_DOWNS      byte = 10
	CANMASKEXPAND_PEDAL_BRAKING_FACTOR              byte = 9
	CANMASKEXPAND_ENGINE_BRAKING_FACTOR             byte = 8
	CANMASKEXPAND_ANALOG_INPUT_VALUE                byte = 7
	CANMASKEXPAND_TACHOGRAHP_DRIVING_DIRECTION      byte = 6
	CANMASKEXPAND_TACHOGRAHP_VEHICLE_MOTION_SIGNAL  byte = 5
	CANMASKEXPAND_TACHOGRAHP_OVERSPEED_SIGNAL       byte = 4
	CANMASKEXPAND_AXLE_WEIGTH_4TH                   byte = 3
	CANMASKEXPAND_AXLE_WEIGTH_3RD                   byte = 2
	CANMASKEXPAND_AXLE_WEIGTH_1ST                   byte = 1
	CANMASKEXPAND_AD_BLUE_LEVEL                     byte = 0

	//eri_mask_bits
	ERIMASK_RF433_ACCESORY_DATA         byte = 7
	ERIMASK_RESERVED_6                  byte = 6
	ERIMASK_RESERVED_5                  byte = 5
	ERIMASK_FUEL_SENSOR_DATA_VOLUME     byte = 4 //Bloque de datos: Fuel Sensor Data (Volumen)
	ERIMASK_FUEL_SENSOR_DATA_PERCENTAGE byte = 3 //Bloque de datos: Fuel Sensor Data (Porcentaje)
	ERIMASK_CAN_DATA                    byte = 2
	ERIMASK_1_WIRE_DATA                 byte = 1
	ERIMASK_DIGITAL_FUEL_SENSOR_DATA    byte = 0
)

type QueclinkGV300W struct {
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

func (g *QueclinkGV300W) GetGpsJsonData() string {
	return ""
}
func (g *QueclinkGV300W) GetData() []byte {
	return nil
}
func (g *QueclinkGV300W) GetComando() string {
	return ""
}
func (g *QueclinkGV300W) GetCodigoGPS() string {
	return ""
}
func (g *QueclinkGV300W) GetAck() []byte {
	return nil
}
func (g *QueclinkGV300W) IsValid() bool {
	return false
}
func (g *QueclinkGV300W) SendACK() bool {
	return false
}
func inc(idx *int) int {
	*idx = *idx + 1
	return *idx
}

func NewQueclinkGV300W(trama string) *QueclinkGV300W {
	datos := strings.Split(trama, ",")
	if len(datos) < 3 {
		return nil
	}
	evento := datos[0][6:]
	codigoGPS := datos[2]
	numeroEvento := eventos[evento]
	var gps QueclinkGV300W
	gps.CodigoGPS = "GL-" + codigoGPS
	gps.Evento = &numeroEvento
	gps.Trama = trama

	switch evento {
	case "GTSTC":
		gps.Velocidad = parseFloat(datos[6])
		gps.Heading = parseInt(datos[7])
		gps.Longitud = parseFloat(datos[9])
		gps.Latitud = parseFloat(datos[10])
		gps.FechaGPS = parseDateLocalTime(datos[11])
	case "GTBTC", "GTEPN", "GTEPF":
		gps.Velocidad = parseFloat(datos[5])
		gps.Heading = parseInt(datos[6])
		gps.Longitud = parseFloat(datos[8])
		gps.Latitud = parseFloat(datos[9])
		gps.FechaGPS = parseDateLocalTime(datos[10])
	case "GTCAN":
		// +RESP:GTCAN,270E01,863457050549706,Soflex,0,1,400A7DFF,,2,H1221530,895.81,1259,24,85,,P44.00,2,904.40,534.22,370.18,166.86,0680,00,,,0,29.8,313,35.1,-58.610707,-34.664033,20250618183702,20250618153704,3697$

		type gpsHandler func()
		i := 0
		Fuel_level_str := ""

		handlersMaskExpand1 := []struct {
			bit     byte
			handler gpsHandler
		}{
			{CANMASKEXPAND_AD_BLUE_LEVEL, func() {
				gps.AdBlueLevel = parseFloat(datos[inc(&i)]) //length<3 (0 - 100) Litros
			}},
			{CANMASKEXPAND_AXLE_WEIGTH_1ST, func() {
				gps.AxleWeight1st = parseFloat(datos[inc(&i)]) //length<=5 (0 - 65535) Kg
			}},
			{CANMASKEXPAND_AXLE_WEIGTH_3RD, func() {
				gps.AxleWeight3rd = parseFloat(datos[inc(&i)]) //length<=5 (0 - 65535) Kg
			}},
			{CANMASKEXPAND_AXLE_WEIGTH_4TH, func() {
				gps.AxleWeight4th = parseFloat(datos[inc(&i)]) //length<=5 (0 - 65535) Kg
			}},
			{CANMASKEXPAND_TACHOGRAHP_OVERSPEED_SIGNAL, func() {
				gps.TachographOverspeedSignal = parseInt(datos[inc(&i)]) //length=1 (0, 1)
			}},
			{CANMASKEXPAND_TACHOGRAHP_VEHICLE_MOTION_SIGNAL, func() {
				gps.TachographVehicleMotionSignal = parseInt(datos[inc(&i)]) //length=1 (0, 1)
			}},
			{CANMASKEXPAND_TACHOGRAHP_DRIVING_DIRECTION, func() {
				gps.TachographDrivingDirection = parseInt(datos[35]) //length=1 (0, 1)
			}},
			{CANMASKEXPAND_ANALOG_INPUT_VALUE, func() {
				gps.AnalogInputValue = parseInt(datos[inc(&i)]) //length<=5 (0 - 999999) mV
			}},
			{CANMASKEXPAND_ENGINE_BRAKING_FACTOR, func() {
				gps.EngineBrakingFactor = parseFloat(datos[inc(&i)]) //length<=6 (0 - 999999)
			}},
			{CANMASKEXPAND_PEDAL_BRAKING_FACTOR, func() {
				gps.PedalBrakingFactor = parseFloat(datos[inc(&i)]) //length<=6 (0 - 999999)
			}},
			{CANMASKEXPAND_TOTAL_ACCELERATOR_KICK_DOWNS, func() {
				gps.TotalAcceleratorKickDowns = parseFloat(datos[inc(&i)]) //length<=6 (0 - 999999)
			}},
			{CANMASKEXPAND_TOTAL_EFFECTIVE_ENGINE_SPEED_TIME, func() {
				gps.TotalEffectiveEngineSpeedTime = parseFloat(datos[inc(&i)]) //length<=10 (0.00 - 99999.99) Horas
			}},
			{CANMASKEXPAND_TOTAL_CRUISE_CONTROL_TIME, func() {
				gps.TotalCruiseControlTime = parseFloat(datos[inc(&i)]) //length<=10 (0.00 - 99999.99) Horas
			}},
			{CANMASKEXPAND_TOTAL_ACCELERATOR_KICK_DOWN_TIME, func() {
				gps.TotalAcceleratorKickDownTime = parseFloat(datos[inc(&i)]) //length<=10 (0.00 - 99999.99) Horas
			}},
			{CANMASKEXPAND_TOTAL_BRAKES_APPLICATIONS, func() {
				gps.TotalBrakesApplications = parseInt(datos[inc(&i)]) //length<=6 (0 - 999999)
			}},
			{CANMASKEXPAND_SERVICE_DISTANCE, func() {
				gps.ServiceDistance = parseInt(datos[inc(&i)]) //length<=5 (0 - 65000)
			}},
			{CANMASKEXPAND_TACHOGRAPH_DRIVER_1_CARD_NUMBER, func() {
				gps.TachographDriver1CardNumber = datos[i] //length<=10 (0 - 9999999999)
			}},
			{CANMASKEXPAND_TACHOGRAPH_DRIVER_2_CARD_NUMBER, func() {
				gps.TachographDriver2CardNumber = datos[i] //length<=10 (0 - 9999999999)
			}},
			{CANMASKEXPAND_TACHOGRAPH_DRIVER_1_NAME, func() {
				gps.TachographDriver1Name = datos[i] //length<=40 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
			}},
			{CANMASKEXPAND_TACHOGRAPH_DRIVER_2_NAME, func() {
				gps.TachographDriver2Name = datos[i] //length<=40 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
			}},
			{CANMASKEXPAND_REGISTRATION_NUMBER, func() {
				gps.RegistrationNumber = datos[i] //length<=10 (0 - 9999999999)
			}},
			{CANMASKEXPAND_EXPANDED_INFORMATION, func() {
				gps.ExpandedInformation = datos[i] //length=4 (0000 - FFFF)
			}},
			{CANMASKEXPAND_RAPID_BRAKINGS, func() {
				gps.RapidBrakings = parseInt(datos[inc(&i)]) //length<=6 (0 - 999999)
			}},
			{CANMASKEXPAND_RAPID_ACCELERATIONS, func() {
				gps.RapidAccelerations = parseInt(datos[inc(&i)]) //length<=6 (0 - 999999)
			}},
		}

		handlersMask1 := []struct {
			bit     byte
			handler gpsHandler
		}{

			{CANMASK_VIN, func() {
				gps.VIN = datos[inc(&i)]
			}},
			{CANMASK_IGNITION_KEY, func() {
				gps.IgnitionKey = parseInt(datos[inc(&i)])
			}},
			{CANMASK_TOTAL_DISTANCE, func() {
				totalDistanceStr := strings.TrimSpace(datos[inc(&i)])
				if totalDistanceStr != "" {
					if totalDistanceStr[0] == 'H' || totalDistanceStr[0] == 'I' {
						gps.TotalDistanceUnits = string(totalDistanceStr[0])
						totalDistanceStr = totalDistanceStr[1:]
					}
					gps.TotalDistance = parseFloat(totalDistanceStr)
				}
			}},
			{CANMASK_TOTAL_FUEL_USED, func() {
				gps.TotalFuelUsed = parseFloat(datos[inc(&i)])
			}},
			{CANMASK_ENGINE_RPM, func() {
				gps.EngineRPM = parseInt(datos[inc(&i)]) //length<=5 (0 - 16383) RPM
			}},
			{CANMASK_VEHICLE_SPEED, func() {
				gps.VehicleSpeed = parseFloat(datos[inc(&i)]) //length<=3 (0 - 455) Km/h
			}},
			{CANMASK_ENGINE_COOLANT_TEMPERATURE, func() {
				gps.EngineCoolantTemperature = parseInt(datos[inc(&i)]) //length<=4 (-40 - +215) ºC
			}},
			{CANMASK_FUEL_CONSUMPTION, func() {
				gps.FuelConsumption = parseFloat(datos[inc(&i)]) //length<=5 (0.0 - 999.9) Litros cada 100 km
			}},
			{CANMASK_FUEL_LEVEL, func() {
				Fuel_level_str = strings.TrimSpace(datos[inc(&i)]) //length<=7 L(0.00 - 9999.99) / P(0.00 - 100.00)

				if Fuel_level_str != "" {
					switch Fuel_level_str[0] {
					case 'L':
						gps.FuelLevelUnits = "L" // Unidades: Litros
					case 'P':
						gps.FuelLevelUnits = "P" // Unidades: Porcentaje
					}
					Fuel_level_str = Fuel_level_str[1:]
					gps.FuelLevel = parseFloat(Fuel_level_str)
				}
			}},
			{CANMASK_RANGE, func() {
				gps.Range = parseFloat(datos[inc(&i)]) //length<=8 (0 - 99999999)
				gps.RangeUnits = "H"
			}},
			{CANMASK_ACCELERATOR_PEDAL_PRESSURE, func() {
				gps.AcceleratorPedalPressure = parseFloat(datos[inc(&i)]) //length<=3 (0 - 100) %
			}},
			{CANMASK_TOTAL_ENGINE_HOURS, func() {
				gps.TotalEngineHours = parseFloat(datos[inc(&i)]) //length<=8 (0.00 - 99999.99) Horas
			}},
			{CANMASK_TOTAL_DRIVING_TIME, func() {
				gps.TotalDrivingTime = parseFloat(datos[inc(&i)]) //length<=8 (0.00 - 99999.99) Horas
			}},
			{CANMASK_TOTAL_ENGINE_IDLE_TIME, func() {
				gps.TotalEngineIdleTime = parseFloat(datos[inc(&i)]) //length<=8 (0.00 - 99999.99) Horas
			}},
			{CANMASK_TOTAL_IDLE_FUEL_USED, func() {
				gps.TotalIdleFuelUsed = parseFloat(datos[inc(&i)]) //length<=9 (0.00 - 99999.99) Litros
			}},

			{CANMASK_AXLE_WEIGHT_2ND, func() {
				gps.AxleWeight2nd = parseInt(datos[inc(&i)]) //length<=5 (0 - 65535) Kg
			}},
			{CANMASK_TACHOGRAPH_INFORMATION, func() {
				gps.TachographInformation = datos[inc(&i)] //length=4 (0000 - FFFF)
			}},
			{CANMASK_DETAILED_INFORMATION, func() {
				gps.DetailedInformation = datos[inc(&i)] //length=4 (0000 - FFFF)
			}},
			{CANMASK_LIGHTS, func() {
				gps.Lights = datos[inc(&i)] //length=2 (00 - FF)
			}},
			{CANMASK_DOORS, func() {
				gps.Doors = datos[inc(&i)] //length=2 (00 - FF)
			}},
			{CANMASK_TOTAL_VEHICLE_OVERSPEED_TIME, func() {
				gps.TotalVehicleOverspeedTime = parseFloat(datos[inc(&i)]) //length<=8 (0.00 - 99999.99) Horas
			}},
			{CANMASK_TOTAL_VEHICLE_ENGINE_OVERSPEED_TIME, func() {
				gps.TotalVehicleEngineOverspeedTime = parseFloat(datos[inc(&i)]) //length<=8 (0.00 - 99999.99) Horas
			}},
			{CANMASK_CAN_REPORT_EXPANSION_MASK, func() {
				gps.CanExpandReportMask = datos[inc(&i)]                          //length=8 (00000000 - FFFFFFFF)
				CanExpandReportMaskBin := HexToBinary(gps.CanExpandReportMask, 4) //Binario de 4 bytes

				//se procesa la primera parte de los datos de la mascara
				for _, h := range handlersMaskExpand1 {
					if BitHabilitado(CanExpandReportMaskBin, h.bit) {
						h.handler()
					}
				}
			}},
		}

		handlersMask2 := []struct {
			bit     byte
			handler gpsHandler
		}{
			{CANMASK_GNSS_INFORMATION, func() {
				gps.GNSSAccuracy = parseInt(datos[inc(&i)]) //length<=2 (0)
				gps.GNSSSpeed = parseFloat(datos[inc(&i)])  //length<=5 (0.0 - 999.9) Km/h
				gps.Azimuth = parseInt(datos[inc(&i)])      //length<=3 (0 - 359)
				gps.Altitude = parseFloat(datos[inc(&i)])   //length<=8 (-xxxxx.x) Metros
				gps.Longitude = parseFloat(datos[inc(&i)])  //length<=11 (-180.0 - +180.0)
				gps.Latitude = parseFloat(datos[inc(&i)])   //length<=10 (-90.0 - +90.0)
				gps.GNSSUTCTime = parseTime(datos[inc(&i)]) //length=14 (yyyyMMddHHmmss)
			}},
			{CANMASK_CELL_INFORMATION, func() {
				gps.MCC = datos[inc(&i)]    //length=4 (0XXX)
				gps.MNC = datos[inc(&i)]    //length=4 (0XXX)
				gps.LAC = datos[inc(&i)]    //length=4 (XXXX)
				gps.CellID = datos[inc(&i)] //length=4-8 (XXXX)
			}},
			{CANMASK_GNSS_INFORMATION, func() {
				if (gps.CanbusDeviceState != nil) && (*gps.CanbusDeviceState == 1) {
					gps.PositionAppendMask = datos[inc(&i)] //length=2 (00, 01)
				}
			}},
		}

		gps.ProtocolVersion = datos[inc(&i)]      //length=6  (XX0000 - XXFFFF, donde XX es 'A'-'Z','0'-'9' )
		gps.UniqueID = datos[inc(&i)]             //length=15 (IMEI)
		gps.DeviceName = datos[inc(&i)]           //length<=20 ('0'-'9' 'a'-'z'  'A'-'Z' '-' '_')
		gps.ReportType = parseInt(datos[inc(&i)]) //length=1 (0, 1, 2)

		gps.CanbusDeviceState = parseInt(datos[inc(&i)])            //length=1 (0, 1)
		gps.CanbusReportMask = datos[inc(&i)]                       //length<=8 (0 - FFFFFFFF)
		CanbusReportMaskBin := HexToBinary(gps.CanbusReportMask, 4) //Binario de 4 bytes

		//se procesa la primera parte de los datos de la mascara
		for _, h := range handlersMask1 {
			if BitHabilitado(CanbusReportMaskBin, h.bit) {
				h.handler()
			}
		}
		// 2. Campos fijos
		gps.Reserved1 = datos[inc(&i)]
		gps.Reserved2 = datos[inc(&i)]

		//se procesa la segunda parte de los datos de la mascara
		for _, h := range handlersMask2 {
			if BitHabilitado(CanbusReportMaskBin, h.bit) {
				h.handler()
			}
		}

		gps.SendTime = parseTime(datos[inc(&i)]) //length=14 (yyyyMMddHHmmss)
		gps.CountNumber = datos[inc(&i)][0:4]    // equivalente a Substring(0, 4)
		gps.TailCharacter = datos[i][4:5]        // equivalente a Substring(4, 1)

		// codigoGPS = UniqueId
		// velocidad = VehicleSpeed
		// longitud = longitude
		// latitud = latitude
		if gps.GNSSUTCTime != nil {
			fechaUtc := gps.GNSSUTCTime.UTC()
			fechaLocal := fechaUtc.Local()
			gps.FechaGPS = &fechaLocal
		}

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
		gps.FechaGPS = parseTime(datos[i])
	case "GTSOS":
		utcNow := time.Now().UTC()
		gps.HDOP = parseFloat(datos[7])
		gps.Velocidad = parseFloat(datos[8])
		gps.Heading = parseInt(datos[9])
		gps.Longitud = parseFloat(datos[11])
		gps.Latitud = parseFloat(datos[12])
		gps.FechaGPS = &utcNow
		gps.Bateria = parseFloat(datos[19])
	case "GTFRI", "GTIGN", "GTIGF", "GTSTT", "GTHBM":
		utcNow := time.Now().UTC()
		gps.FechaGPS = &utcNow
	default:
		gps.HDOP = parseFloat(datos[7])
		gps.Velocidad = parseFloat(datos[8])
		gps.Heading = parseInt(datos[9])
		gps.Longitud = parseFloat(datos[11])
		gps.Latitud = parseFloat(datos[12])
		gps.FechaGPS = parseTime(datos[13])
		gps.Bateria = parseFloat(datos[19])
	}

	// Enviar al bulk
	fmt.Printf("POS: %+v\n", gps)
	return &gps
}
