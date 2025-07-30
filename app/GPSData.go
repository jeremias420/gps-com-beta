package app

import "time"

type GPSData struct {
	Prefijo      string
	CodigoGPS    string
	Evento       *int
	NumeroEvento *int
	Trama        string

	HDOP      *float64 // Horizontal Dilution of Precision
	Velocidad *float64
	Heading   *int
	Longitud  *float64
	Latitud   *float64
	FechaGPS  *time.Time
	Bateria   *float64
	FechaUtc  *time.Time

	// Canbus ------------------------------------
	ProtocolVersion          string
	UniqueID                 string
	DeviceName               string
	ReportType               *int     //Tipo de reporte (0: reporte periodico, 1: pedido de reporte en tiempo real, 2 reporte de ignicion on/off)
	CanbusDeviceState        *int     //Estado de comunicacion con el dispositivo canbus externo (0: Anormal-falla para recibir datos, 1: Normal-habilitado para recibir datos)
	CanbusReportMask         string   //Consultar <CAN Report Mask> en AT+GTCAN
	VIN                      string   //Reservado en dispositivos CAN100
	IgnitionKey              *int     //Estado de la ingnición (0: ignición off, 1: ignición on, 2: motor on)
	TotalDistance            *float64 //Distancia total recorrida por el vehiculo ("H" Hectometros ó "I" Impulso de Distancia, si la distancia no esta disponible en el panel)
	TotalDistanceUnits       string   //Dato adicional local. Unidades de la distancia total recorrida del vehiculo ("H" ó "I")
	TotalFuelUsed            *float64 //Cantidad de combustible utilizado desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades en Litros.
	EngineRPM                *int     //Revoluciones por minuto del motor
	VehicleSpeed             *float64 //Velocidad del vehiculo. Unidades Km/h
	EngineCoolantTemperature *int     //Temperatura de motor. Unidades ºC
	FuelConsumption          *float64 //Consumo de combustible. Unidades Litros/100km
	FuelLevel                *float64 //Nivel de combustible en el tanque. Unidades "L" Litros ó "P" Porcentaje
	FuelLevelUnits           string   //Dato adicional local. Unidades del nivel de combustible.
	Range                    *float64 //Cantidad de hectometros a recorrer con el combustible restante. Unidades
	RangeUnits               string   //Dato adicional local. Unidades de range ("H" = Hectometros)
	AcceleratorPedalPressure *float64 //Presion del acelerados. Unidades % Porcentaje
	TotalEngineHours         *float64 //Tiempo total de motor funcionando desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
	TotalDrivingTime         *float64 //Tiempo total de motor funcionando en velocidad mayor a cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
	TotalEngineIdleTime      *float64 //Tiempo total de motor funcionando en velocidad cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
	TotalIdleFuelUsed        *float64 //Cantidad de combustible utilizado con motor funcionando en velocidad cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Litros.
	AxleWeight2nd            *int     //Peso del segundo eje del vehiculo. Unidades Kg
	TachographInformation    string   //Dos bytes. El byte alto describe el tacografo del driver 2 y el byte bajo describe el el tacografo del driver 1
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
	DetailedInformation string // Numero hexadecimal. Cada bit contiene información de un indicador
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
	Lights string // Numero hexadecimal. Cada bit contiene información de una luz particular
	/*Bit 0: Luces de circulación diurna (1: encendidas, 0:  apagadas)
	  Bit 1: Luces bajas (1: encendidas, 0:  apagadas)
	  Bit 2: Luces altas (1: encendidas, 0: apagadas)
	  Bit 3: Luz antiniebla delantera (1: encendida, 0: apagada)
	  Bit 4: Luz antiniebla trasera (1: encendida, 0: apagada)
	  Bit 5: Balizas (1: encendidas, 0: apagadas)
	  Bit 6: Reservado
	  Bit 7: Reservado
	*/
	Doors string // Numero hexadecimal. Cada bit contiene información de una puerta particular
	/*Bit 0: Puerta del conductor (1: abierta, 0: cerrada)
	  Bit 1: Puerta del pasajero (1: abierta, 0: cerrada)
	  Bit 2: Puerta trasera izquierda (1: abierta, 0: cerrada)
	  Bit 3: Puerta trasera derecha (1: abierta, 0: cerrada)
	  Bit 4: Baul (1: abierto, 0: cerrado)
	  Bit 5: Capó (1: abierto, 0: cerrado)
	  Bit 6: Reservado
	  Bit 7: Reservado
	*/
	TotalVehicleOverspeedTime       *float64
	TotalVehicleEngineOverspeedTime *float64
	CanExpandReportMask             string
	AdBlueLevel                     *float64
	AxleWeight1st                   *float64
	AxleWeight3rd                   *float64
	AxleWeight4th                   *float64
	TachographOverspeedSignal       *int
	TachographVehicleMotionSignal   *int
	TachographDrivingDirection      *int
	AnalogInputValue                *int
	EngineBrakingFactor             *float64
	PedalBrakingFactor              *float64
	TotalAcceleratorKickDowns       *float64
	Reserved1                       string
	Reserved2                       string
	GNSSAccuracy                    *int
	GNSSSpeed                       *float64
	Azimuth                         *int
	Altitude                        *float64 // Altura sobre el nivel del mar. En metros
	Longitude                       *float64
	Latitude                        *float64
	GNSSUTCTime                     *time.Time // GNSS=Global Navigation Satellite System; UTC=Coordinated Universal Time
	MCC                             string
	MNC                             string
	LAC                             string
	CellID                          string
	PositionAppendMask              string
	SendTime                        *time.Time
	CountNumber                     string
	TailCharacter                   string
	TotalEffectiveEngineSpeedTime   *float64
	TotalCruiseControlTime          *float64
	TotalAcceleratorKickDownTime    *float64
	TotalBrakesApplications         *int
	ServiceDistance                 *int
	TachographDriver1CardNumber     string
	TachographDriver2CardNumber     string
	TachographDriver1Name           string
	TachographDriver2Name           string
	RegistrationNumber              string
	ExpandedInformation             string
	RapidBrakings                   *int
	RapidAccelerations              *int
}
