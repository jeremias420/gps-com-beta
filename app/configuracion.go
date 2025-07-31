package app

import (
	"encoding/json"
	"io"
	"os"
	"path/filepath"
)

var Config Configuracion

// Configuracion Estructura que almacena configuracion de la aplicacion
type Configuracion struct {
	Port       int    `json:"port"`
	Tecnologia string `json:"tecnologia"`
	Reenvio    struct {
		Enabled bool   `json:"enabled"`
		Host    string `json:"host"`
		Format  string `json:"format"`
	} `json:"reenvio"`
	EnviarACK        bool `json:"enviarACK"`
	LocalizacionFija bool `json:"localizacion_fija"`
}

func GetCurrentDir() (string, error) {
	// Obtener la ruta actual del directorio de trabajo
	currentDir, err := os.Getwd()
	if err != nil {
		return "", err
	}
	return currentDir, nil
}

func LoadConfig() (Configuracion, error) {
	currentDir := obtenerDirectorioBase()

	// Concatenar el nombre del archivo config.json a la ruta actual
	configFilePath := filepath.Join(currentDir, "config.json")

	jsonFile, err := os.Open(configFilePath)
	if err != nil {
		return Configuracion{}, err
	}
	defer jsonFile.Close()

	byteValue, _ := io.ReadAll(jsonFile)

	var config Configuracion

	json.Unmarshal([]byte(byteValue), &config)

	if config.Port == 0 {
		config.Port = 8080
	}

	return config, nil
}
