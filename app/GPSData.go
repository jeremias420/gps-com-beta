package app

import "time"

type GPSData struct {
	Prefijo      string `json:"prefijo"`
	CodigoGPS    string `json:"codigo_gps"`
	Evento       *int   `json:"evento,omitempty"`
	NumeroEvento *int   `json:"numero_evento,omitempty"`
	Trama        string `json:"trama"`

	HDOP      *float64   `json:"hdop,omitempty"` // Horizontal Dilution of Precision
	Velocidad *float64   `json:"velocidad,omitempty"`
	Heading   *int       `json:"heading,omitempty"`
	Longitud  *float64   `json:"longitud,omitempty"`
	Latitud   *float64   `json:"latitud,omitempty"`
	FechaGPS  *time.Time `json:"fecha_gps,omitempty"`
	Bateria   *float64   `json:"bateria,omitempty"`
	FechaUtc  *time.Time `json:"fecha_utc,omitempty"`
	ContactId *int       `json:"contact_id,omitempty"`

	// Canbus ------------------------------------
	ProtocolVersion          string   `json:"protocol_version"`
	UniqueID                 string   `json:"unique_id"`
	DeviceName               string   `json:"device_name"`
	ReportType               *int     `json:"report_type,omitempty"`                //Tipo de reporte (0: reporte periodico, 1: pedido de reporte en tiempo real, 2 reporte de ignicion on/off)
	CanbusDeviceState        *int     `json:"canbus_device_state,omitempty"`        //Estado de comunicacion con el dispositivo canbus externo (0: Anormal-falla para recibir datos, 1: Normal-habilitado para recibir datos)
	CanbusReportMask         string   `json:"canbus_report_mask"`                   //Consultar <CAN Report Mask> en AT+GTCAN
	VIN                      string   `json:"vin"`                                  //Reservado en dispositivos CAN100
	IgnitionKey              *int     `json:"ignition_key,omitempty"`               //Estado de la ingnición (0: ignición off, 1: ignición on, 2: motor on)
	TotalDistance            *float64 `json:"total_distance,omitempty"`             //Distancia total recorrida por el vehiculo ("H" Hectometros ó "I" Impulso de Distancia, si la distancia no esta disponible en el panel)
	TotalDistanceUnits       string   `json:"total_distance_units"`                 //Dato adicional local. Unidades de la distancia total recorrida del vehiculo ("H" ó "I")
	TotalFuelUsed            *float64 `json:"total_fuel_used,omitempty"`            //Cantidad de combustible utilizado desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades en Litros.
	EngineRPM                *int     `json:"engine_rpm,omitempty"`                 //Revoluciones por minuto del motor
	VehicleSpeed             *float64 `json:"vehicle_speed,omitempty"`              //Velocidad del vehiculo. Unidades Km/h
	EngineCoolantTemperature *int     `json:"engine_coolant_temperature,omitempty"` //Temperatura de motor. Unidades ºC
	FuelConsumption          *float64 `json:"fuel_consumption,omitempty"`           //Consumo de combustible. Unidades Litros/100km
	FuelLevel                *float64 `json:"fuel_level,omitempty"`                 //Nivel de combustible en el tanque. Unidades "L" Litros ó "P" Porcentaje
	FuelLevelUnits           string   `json:"fuel_level_units"`                     //Dato adicional local. Unidades del nivel de combustible.
	Range                    *float64 `json:"range,omitempty"`                      //Cantidad de hectometros a recorrer con el combustible restante. Unidades
	RangeUnits               string   `json:"range_units"`                          //Dato adicional local. Unidades de range ("H" = Hectometros)
	AcceleratorPedalPressure *float64 `json:"accelerator_pedal_pressure,omitempty"` //Presion del acelerados. Unidades % Porcentaje
	TotalEngineHours         *float64 `json:"total_engine_hours,omitempty"`         //Tiempo total de motor funcionando desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
	TotalDrivingTime         *float64 `json:"total_driving_time,omitempty"`         //Tiempo total de motor funcionando en velocidad mayor a cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
	TotalEngineIdleTime      *float64 `json:"total_engine_idle_time,omitempty"`     //Tiempo total de motor funcionando en velocidad cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Horas.
	TotalIdleFuelUsed        *float64 `json:"total_idle_fuel_used,omitempty"`       //Cantidad de combustible utilizado con motor funcionando en velocidad cero desde que el vehiculo fue fabricado o desde que se instaló el dispositivo. Unidades Litros.
	AxleWeight2nd            *int     `json:"axle_weight_2nd,omitempty"`            //Peso del segundo eje del vehiculo. Unidades Kg
	TachographInformation    string   `json:"tachograph_information"`               //Dos bytes. El byte alto describe el tacografo del driver 2 y el byte bajo describe el el tacografo del driver 1
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
	TotalVehicleOverspeedTime       *float64   `json:"total_vehicle_overspeed_time,omitempty"`
	TotalVehicleEngineOverspeedTime *float64   `json:"total_vehicle_engine_overspeed_time,omitempty"`
	CanExpandReportMask             string     `json:"can_expand_report_mask"`
	AdBlueLevel                     *float64   `json:"adblue_level,omitempty"`
	AxleWeight1st                   *float64   `json:"axle_weight_1st,omitempty"`
	AxleWeight3rd                   *float64   `json:"axle_weight_3rd,omitempty"`
	AxleWeight4th                   *float64   `json:"axle_weight_4th,omitempty"`
	TachographOverspeedSignal       *int       `json:"tachograph_overspeed_signal,omitempty"`
	TachographVehicleMotionSignal   *int       `json:"tachograph_vehicle_motion_signal,omitempty"`
	TachographDrivingDirection      *int       `json:"tachograph_driving_direction,omitempty"`
	AnalogInputValue                *int       `json:"analog_input_value,omitempty"`
	EngineBrakingFactor             *float64   `json:"engine_braking_factor,omitempty"`
	PedalBrakingFactor              *float64   `json:"pedal_braking_factor,omitempty"`
	TotalAcceleratorKickDowns       *float64   `json:"total_accelerator_kick_downs,omitempty"`
	Reserved1                       string     `json:"reserved_1"`
	Reserved2                       string     `json:"reserved_2"`
	GNSSAccuracy                    *int       `json:"gnss_accuracy,omitempty"`
	GNSSSpeed                       *float64   `json:"gnss_speed,omitempty"`
	Azimuth                         *int       `json:"azimuth,omitempty"`
	Altitude                        *float64   `json:"altitude,omitempty"` // Altura sobre el nivel del mar. En metros
	Longitude                       *float64   `json:"longitude,omitempty"`
	Latitude                        *float64   `json:"latitude,omitempty"`
	GNSSUTCTime                     *time.Time `json:"gnss_utc_time,omitempty"` // GNSS=Global Navigation Satellite System; UTC=Coordinated Universal Time
	MCC                             string     `json:"mcc"`
	MNC                             string     `json:"mnc"`
	LAC                             string     `json:"lac"`
	CellID                          string     `json:"cell_id"`
	PositionAppendMask              string     `json:"position_append_mask"`
	SendTime                        *time.Time `json:"send_time,omitempty"`
	CountNumber                     string     `json:"count_number"`
	TailCharacter                   string     `json:"tail_character"`
	TotalEffectiveEngineSpeedTime   *float64   `json:"total_effective_engine_speed_time,omitempty"`
	TotalCruiseControlTime          *float64   `json:"total_cruise_control_time,omitempty"`
	TotalAcceleratorKickDownTime    *float64   `json:"total_accelerator_kick_down_time,omitempty"`
	TotalBrakesApplications         *int       `json:"total_brakes_applications,omitempty"`
	ServiceDistance                 *int       `json:"service_distance,omitempty"`
	TachographDriver1CardNumber     string     `json:"tachograph_driver_1_card_number"`
	TachographDriver2CardNumber     string     `json:"tachograph_driver_2_card_number"`
	TachographDriver1Name           string     `json:"tachograph_driver_1_name"`
	TachographDriver2Name           string     `json:"tachograph_driver_2_name"`
	RegistrationNumber              string     `json:"registration_number"`
	ExpandedInformation             string     `json:"expanded_information"`
	RapidBrakings                   *int       `json:"rapid_brakings,omitempty"`
	RapidAccelerations              *int       `json:"rapid_accelerations,omitempty"`
}
