package app

import (
	"fmt"
	"os"
	"path/filepath"
	"strconv"
	"time"
)

func obtenerDirectorioBase() string {
	exePath, err := os.Executable()
	if err != nil {
		return "."
	}
	return filepath.Dir(exePath)
}

// Helpers
func strOrEmpty(p *string) string {
	if p != nil {
		return *p
	}
	return ""
}

func intToStr(p *int) string {
	if p != nil {
		return strconv.Itoa(*p)
	}
	return ""
}

func floatStrToIntStr(p *string) string {
	if p != nil {
		if f, err := strconv.ParseFloat(*p, 64); err == nil {
			return strconv.Itoa(int(f)) // trunca parte decimal
		}
	}
	return "0"
}

func float64OrZeroFromStr(p *string) float64 {
	if p != nil {
		if val, err := strconv.ParseFloat(*p, 64); err == nil {
			return val
		}
	}
	return 0
}

func convertirFechaCompacta(fechaStr string) string {
	if len(fechaStr) != 14 {
		return ""
	}
	t, err := time.Parse("20060102150405", fechaStr)
	if err != nil {
		return ""
	}
	return t.Format("20060102 15:04:05")
}

func floatToStr(v *float64) string {
	if v == nil {
		return ""
	}
	return fmt.Sprintf("%.6f", *v)
}

func strOrNil(s *string) string {
	if s == nil {
		return ""
	}
	return *s
}

func parseFloat(s string) *float64 {
	if s == "" {
		return nil
	}
	val, err := strconv.ParseFloat(s, 64)
	if err != nil {
		return nil
	}
	return &val
}

func parseDateLocalTime(val string) *time.Time {
	if val == "" {
		return nil
	}
	t, err := time.Parse("20060102150405", val) // yyyyMMddHHmmss
	if err != nil {
		return nil
	}
	t = t.Local()
	return &t
}

func parseTime(val string) *time.Time {
	if val == "" {
		return nil
	}
	t, err := time.Parse("20060102150405", val) // yyyyMMddHHmmss
	if err != nil {
		return nil
	}

	return &t
}

func parseTimeToString(t time.Time) string {
	return t.Format("20060102150405")
}


func parseInt(s string) *int {
	if s == "" {
		return nil
	}
	val, err := strconv.Atoi(s)
	if err != nil {
		return nil
	}
	return &val
}

func parseDateTimeUTC(s string) *time.Time {
	if s == "" {
		return nil
	}
	t, err := time.ParseInLocation("20060102150405", s, time.UTC)
	if err != nil {
		return nil
	}
	return &t
}
