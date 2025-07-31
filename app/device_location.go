package app

import (
	"encoding/json"
	"log"
	"os"
)

type device_location struct {
	ID        string   `json:"id"`
	Latitude  *float64 `json:"latitude,omitempty"`
	Longitude *float64 `json:"longitude,omitempty"`
}

func getDeviceLocationFromJson(id string) *device_location {
	r := &device_location{}
	if Config.LocalizacionFija {
		file := "device_location.json"
		if _, err := os.Stat(file); err == nil {
			jsonData, err := os.ReadFile(file)
			if err == nil {
				var locations []device_location
				err = json.Unmarshal(jsonData, &locations)
				if err == nil {
					for _, location := range locations {
						if location.ID == id {
							r = &location
							break
						}
					}
				} else {
					log.Println("json location error: ", err.Error())
				}
			} else {
				log.Println("file read error: ", err.Error())
			}
		}
		return nil
	}
	return r
}
