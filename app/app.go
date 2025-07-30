package app

import (
	"net"
	"os"
	"strconv"
	"strings"
	"time"

	// log "github.com/soflex/go-package-log"
)

const (
	maxPacketLen = 65535
)

func ProcessDataGL300(data string) {
	// parsed := GetData(data)
	// if Config.Reenvio.Enabled {

	// }

}

func Start() {

	//cargar configuracion
	var err error
	Config, err = LoadConfig()
	if err != nil {
		// log.Error("main", "LoadConfig", err.Error())
		os.Exit(1)
	}

	// log.Info("main", "inicio", "incio de servicio "+time.Now().Format("2006-01-02T15:04:05"))

	//configurar puerto UDP
	listenAddr := ":" + strconv.Itoa(Config.Port)
	addr, err := net.ResolveUDPAddr("udp", listenAddr)
	if err != nil {
		// log.Error("app", "Start", "Error resolviendo dirección UDP: "+err.Error())
	}

	//Escucha el puerto UDP configurado
	conn, err := net.ListenUDP("udp", addr)
	if err != nil {
		// log.Error("app", "Start", "Error escuchando UDP: "+err.Error())
	}
	defer conn.Close()

	// log.Debug("app", "Start", "Escuchando en: "+listenAddr)

	for {
		//comienza a leer datos del puerto UDP
		buf := make([]byte, maxPacketLen)
		n, _, err := conn.ReadFromUDP(buf)
		if err != nil {
			// log.Error("app", "Start", "Error leyendo UDP: "+err.Error())
			continue
		}

		data := string(buf[:n])

		var tecno TecnologiaGPS
		// procesar segun tecnologia seteada en la configuracion
		switch strings.ToLower(Config.Tecnologia) {
		case "gl300":
			tecno = NewQueclinkGV300W(data)

		}
		tecno.GetData()
		// tecno.GetAck()
		// if Config.EnviarACK {
		// 	tecno.SendAck()
		// }
	}
}

//todo : obtener del udp la ip y puerto de origen para enviar el ack
