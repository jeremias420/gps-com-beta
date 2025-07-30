package app

import (
	"encoding/json"
	"time"
)

// * Paquete
type Paquete struct {
	Trama string `json:"trama,omitempty"`
}

func (p *Paquete) EstaVacio() bool {
	return p.Trama == ""
}

// * Movil
type Movil struct {
	IDEquipoAvle    string `json:"id_equipo_avle,omitempty"`
	IDMovilAvle     string `json:"id_movil_avle,omitempty"`
	IDEquipoMapsoft string `json:"id_equipo_mapsoft,omitempty"`
	IDMovilMapsoft  string `json:"id_movil_mapsoft,omitempty"`
}

func (m *Movil) EstaVacio() bool {
	return m.IDEquipoAvle == "" && m.IDMovilAvle == "" && m.IDEquipoMapsoft == "" && m.IDMovilMapsoft == ""
}

// * GPS
type GPS struct {
	IDGPS            string `json:"id_gps,omitempty"`
	IDGPSPrefijo     string `json:"id_gps_prefijo,omitempty"`
	FechaGPSUTC      string `json:"fecha_gps_utc,omitempty"`
	FechaGPSLocal    string `json:"fecha_gps_local,omitempty"`
	FechaServerLocal string `json:"fecha_server_local,omitempty"`
	Lat              string `json:"lat,omitempty"`
	Lon              string `json:"lon,omitempty"`
	Alt              string `json:"alt,omitempty"`
	Rumbo            string `json:"rumbo,omitempty"`
	IDEvento         string `json:"id_evento,omitempty"`
	VelGPS           string `json:"vel_gps,omitempty"`
	Sat              string `json:"sat,omitempty"`
	Acc              string `json:"acc,omitempty"`
	Azim             string `json:"azim,omitempty"`
	RFID             string `json:"rfid,omitempty"`
	IDEventoExtra    string `json:"id_evento_extra,omitempty"`
	HDOP             string `json:"hdop,omitempty"`
	Analog1          string `json:"analog1,omitempty"`
	Analog2          string `json:"analog2,omitempty"`
	Analog3          string `json:"analog3,omitempty"`
	Analog4          string `json:"analog4,omitempty"`
	Analog5          string `json:"analog5,omitempty"`
	Analog6          string `json:"analog6,omitempty"`
	Analog7          string `json:"analog7,omitempty"`
	Analog8          string `json:"analog8,omitempty"`
	Analog9          string `json:"analog9,omitempty"`
	EstadoGPS        string `json:"estado_gps,omitempty"`
	DigitalIn        string `json:"digital_in,omitempty"`
}

func (g *GPS) EstaVacio() bool {
	return *g == GPS{}
}

// * Canbus
type Canbus struct {
	Vel            string `json:"vel,omitempty"`
	Bat            string `json:"bat,omitempty"`
	Ign            string `json:"ign,omitempty"`
	Odo            string `json:"odo,omitempty"`
	CombTotal      string `json:"comb_total,omitempty"`
	CombTotalP     string `json:"comb_total_p,omitempty"`
	RPM            string `json:"rpm,omitempty"`
	Temp           string `json:"temp,omitempty"`
	ConsumoComb    string `json:"consumo_comb,omitempty"`
	Comb           string `json:"comb,omitempty"`
	CombP          string `json:"comb_p,omitempty"`
	Acel           string `json:"acel,omitempty"`
	TMotorFunc     string `json:"tmotor_func,omitempty"`
	TMotorFuncVel  string `json:"tmotor_func_vel,omitempty"`
	TMotorFuncVel0 string `json:"tmotor_func_vel0,omitempty"`
	CombVel0       string `json:"comb_vel0,omitempty"`
	PresAceite     string `json:"pres_aceite,omitempty"`
}

func (c *Canbus) EstaVacio() bool {
	return *c == Canbus{}
}

// * GpsJsonData
type GpsJsonData struct {
	Paquete *Paquete `json:"paquete,omitempty"`
	Movil   *Movil   `json:"movil,omitempty"`
	GPS     *GPS     `json:"gps,omitempty"`
	Canbus  *Canbus  `json:"canbus,omitempty"`
}

func (g *GpsJsonData) GetDataJson() string {
	data, _ := json.Marshal(g)
	return string(data)
}

func (g *GpsJsonData) SetData(
	trama string,
	idEquipoAvle, idMovilAvle, idEquipoMapsoft, idMovilMapsoft *int,
	idGPS, idGPSPrefijo string,
	fechaUTC, fechaLocal, fechaServer time.Time,
	lat, lon, alt *float64,
	rumbo, idEvento *int,
	velGPS *float64,
	sat *int,
	acc *float64,
	azim *int,
	rfid *string,
	idEventoExtra *int,
	hdop,
	analog1,
	analog2,
	analog3,
	analog4,
	analog5,
	analog6,
	analog7,
	analog8,
	analog9 *float64,
	estadoGPS *float64,
	digitalIn *int,
	vel, bat *float64,
	ign *int,
	odo, combTotal, combTotalP *float64,
	rpm, temp *int,
	consumoComb, comb, combP, acel, tmotorFunc, tmotorFuncVel, tmotorFuncVel0, combVel0, presAceite *float64,
) {
	if trama != "" {
		g.Paquete = &Paquete{Trama: trama}
	}
	g.Movil = &Movil{
		IDEquipoAvle:    intToStr(idEquipoAvle),
		IDMovilAvle:     intToStr(idMovilAvle),
		IDEquipoMapsoft: intToStr(idEquipoMapsoft),
		IDMovilMapsoft:  intToStr(idMovilMapsoft),
	}
	if g.Movil.EstaVacio() {
		g.Movil = nil
	}
	g.GPS = &GPS{
		IDGPS:            idGPS,
		IDGPSPrefijo:     idGPSPrefijo,
		FechaGPSUTC:      fechaUTC.Format("20060102150405"),
		FechaGPSLocal:    fechaLocal.Format("20060102150405"),
		FechaServerLocal: fechaServer.Format("20060102150405"),
		Lat:              floatToStr(lat),
		Lon:              floatToStr(lon),
		Alt:              floatToStr(alt),
		Rumbo:            intToStr(rumbo),
		IDEvento:         intToStr(idEvento),
		VelGPS:           floatToStr(velGPS),
		Sat:              intToStr(sat),
		Acc:              floatToStr(acc),
		Azim:             intToStr(azim),
		RFID:             strOrNil(rfid),
		IDEventoExtra:    intToStr(idEventoExtra),
		HDOP:             floatToStr(hdop),
		Analog1:          floatToStr(analog1),
		Analog2:          floatToStr(analog2),
		Analog3:          floatToStr(analog3),
		Analog4:          floatToStr(analog4),
		Analog5:          floatToStr(analog5),
		Analog6:          floatToStr(analog6),
		Analog7:          floatToStr(analog7),
		Analog8:          floatToStr(analog8),
		Analog9:          floatToStr(analog9),
		EstadoGPS:        floatToStr(estadoGPS),
		DigitalIn:        intToStr(digitalIn),
	}
	if g.GPS.EstaVacio() {
		g.GPS = nil
	}
	g.Canbus = &Canbus{
		Vel:            floatToStr(vel),
		Bat:            floatToStr(bat),
		Ign:            intToStr(ign),
		Odo:            floatToStr(odo),
		CombTotal:      floatToStr(combTotal),
		CombTotalP:     floatToStr(combTotalP),
		RPM:            intToStr(rpm),
		Temp:           intToStr(temp),
		ConsumoComb:    floatToStr(consumoComb),
		Comb:           floatToStr(comb),
		CombP:          floatToStr(combP),
		Acel:           floatToStr(acel),
		TMotorFunc:     floatToStr(tmotorFunc),
		TMotorFuncVel:  floatToStr(tmotorFuncVel),
		TMotorFuncVel0: floatToStr(tmotorFuncVel0),
		CombVel0:       floatToStr(combVel0),
		PresAceite:     floatToStr(presAceite),
	}
	if g.Canbus.EstaVacio() {
		g.Canbus = nil
	}
}
