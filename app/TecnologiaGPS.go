package app

type TecnologiaGPS interface {
	GetGpsJsonData() string
	GetData() []byte
	GetComando() string
	GetCodigoGPS() string
	GetAck() []byte
	IsValid() bool
	SendACK() bool
}
