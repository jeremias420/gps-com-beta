package app

import (
	"encoding/json"
	"fmt"
	"log"
	"strconv"
	"strings"
	"time"
)

type HexacomACUDAV410 struct {
	GPSData
}

var Prefijo string

func (g *HexacomACUDAV410) GetGpsJsonData() string {
	return ""
}
func (g *HexacomACUDAV410) GetDataLog() ([]byte, error) {
	aux := struct {
	}{}
	return json.Marshal(aux)
}
func (g *HexacomACUDAV410) GetComando() string {
	return ""
}
func (g *HexacomACUDAV410) GetAck() []byte {
	return nil
}
func (g *HexacomACUDAV410) IsValid() bool {
	return false
}
func (g *HexacomACUDAV410) SendACK() bool {
	return false
}
func (g *HexacomACUDAV410) reenviarDestinoGenerico() {
	datos := "<Soflex>" +
		"trackingid=" + g.CodigoGPS + ";" +
		"date=" + g.FechaGPS.UTC().Format("20060102150405") + ";" +
		"lat=" + strconv.FormatFloat(*g.Latitud, 'f', -1, 64) + ";" +
		"lon=" + strconv.FormatFloat(*g.Longitud, 'f', -1, 64) + ";" +
		"speed=" + strconv.FormatFloat(*g.Velocidad, 'f', -1, 64) + ";" +
		"bearing=" + strconv.Itoa(*g.Heading) + ";" +
		"event=" + strconv.Itoa(*g.NumeroEvento)

	envio := []byte(datos)
	fmt.Println("Datos a reenviar:", string(envio))
	// TODO: Implementar el envío de datos al destino Generico
	/* Código de c# a adaptar
	udpSocket.Send(envio, envio.Length, ConfigurationManager.AppSettings["IPDestinoGenerico"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["PuertoDestinoGenerico"]));
	Console.WriteLine("Reenviado: " + datos);
	*/
}
func (g *HexacomACUDAV410) reenviarDestinoRGP() {
	datos := ">RPF" +
		g.FechaGPS.UTC().Format("020106150405") +
		(strings.ReplaceAll(fmt.Sprintf("%.8f", *g.Latitud), ".", "") + "00000000")[:8] +
		"-" + ("0" + strings.ReplaceAll(strings.ReplaceAll(fmt.Sprintf("%.9f", *g.Longitud), "-", ""), ".", "") + "000000000")[:9] +
		("000" + strconv.Itoa(int(*g.Velocidad*0.621371)))[len("000"+strconv.Itoa(int(*g.Velocidad*0.621371)))-3:] +
		("000" + strconv.Itoa(*g.Heading))[len("000"+strconv.Itoa(*g.Heading))-3:] +
		"0" + "00" + "00" +
		("00" + strconv.Itoa(*g.NumeroEvento))[len("00"+strconv.Itoa(*g.NumeroEvento))-2:] +
		";ID=" + g.CodigoGPS +
		";#0002;*52<"

	envio := []byte(datos)
	fmt.Println("Datos a reenviar:", string(envio))
	// TODO: Implementar el envío de datos al destino RGP
	/* Código de c# a adaptar
	udpSocket.Send(envio, envio.Length, ConfigurationManager.AppSettings["IPDestinoRGP"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["PuertoDestinoRGP"]));
	Console.WriteLine("Reenviado: " + datos);
	*/
}

func (g *HexacomACUDAV410) GetCodigoGPS() string {
	//cadena = PPCCCCQQEEEGGZZZ (16 caracteres)
	CCCC := g.Trama[2:4]  //CCCC = ID de Abonado (Numero de abonado)
	ZZZ := g.Trama[13:16] //ZZZ = ID de Zona (Identificador de dispositivo)

	result := CCCC + ZZZ
	if Prefijo != "" && result != "" {
		result = Prefijo + result
	}

	return result
}

func LimpiarPaquete(trama string) string {
	cadena := strings.ReplaceAll(strings.TrimSpace(trama), " ", "")
	return cadena
}

func NewHexacomACUDAV410(trama string) *HexacomACUDAV410 {
	var gps HexacomACUDAV410

	datos := strings.Split(trama, ";")
	fmt.Println("Datos recibidos:", datos)
	gps.Trama = trama
	gps.CodigoGPS = gps.GetCodigoGPS()

	auxFecha := time.Now()
	gps.FechaGPS = &auxFecha

	CCCC := trama[2:4]
	ZZZ := trama[13:16]
	QQ := trama[6:8]
	EEE := trama[8:11]
	EventoStr := QQ + EEE

	nroEvento, _ := strconv.Atoi(EventoStr)
	gps.NumeroEvento = &nroEvento

	if gps.Latitud == nil && gps.Longitud == nil {
		dlocation := getDeviceLocationFromJson(CCCC)
		if dlocation != nil {
			if dlocation.Latitude != nil && dlocation.Longitude != nil {
				gps.Latitud = dlocation.Latitude
				gps.Longitud = dlocation.Longitude
			} else {
				log.Println("Latitud o Longitud no disponibles en device_location.json")
			}
		} else {
			log.Println("No se encontró ubicación para el dispositivo con ID:", CCCC)
		}
	}

	ParseData := "CodigoGPS=" + gps.CodigoGPS + " " +
		"ID_Abogad:" + CCCC + " " +
		"ID_Zona:" + ZZZ + " " +
		"lat: " + strings.Replace(fmt.Sprintf("%f", *gps.Latitud), ",", ".", -1) + " " +
		"lon: " + strings.Replace(fmt.Sprintf("%f", *gps.Longitud), ",", ".", -1)
	fmt.Println("ParseData:", ParseData)
	if ZZZ != "000" {
		// Implemetar reenvios
	}
	return &gps
}
