package app

type TecnologiaGPS interface {
	GetGpsJsonData() string
	GetDataLog() ([]byte, error)
	GetComando() string
	GetCodigoGPS() string
	GetAck() []byte
	IsValid() bool
	SendACK() bool
}
