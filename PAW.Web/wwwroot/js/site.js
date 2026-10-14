// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function loadExchangeRate() {
    const cacheKey = "exchangeRateCache";
    const cacheDuration = 30 * 60 * 1000; // 30 minutos

    const buyElement = document.getElementById("dollar-buy");
    const sellElement = document.getElementById("dollar-sell");
    const dateElement = document.getElementById("exchange-rate-date");
    const statusElement = document.getElementById("exchange-rate-status");

    if (!buyElement || !sellElement) return;

    try {
        // Revisar si tenemos datos guardados en caché
        const cachedData = localStorage.getItem(cacheKey);

        if (cachedData) {
            const cache = JSON.parse(cachedData);
            const cacheIsValid =
                Date.now() - cache.timestamp < cacheDuration;

            if (cacheIsValid) {
                displayExchangeRate(cache.data, dateElement, buyElement,
                    sellElement, statusElement, true);
                return;
            }
        }

        statusElement.textContent = "Consultando tipo de cambio...";

        const response = await fetch(
            "https://api.hacienda.go.cr/indicadores/tc"
        );

        if (!response.ok) {
            throw new Error("No se pudo consultar el servicio.");
        }

        const data = await response.json();

        // Guardar los datos durante 30 minutos
        localStorage.setItem(cacheKey, JSON.stringify({
            timestamp: Date.now(),
            data: data
        }));

        displayExchangeRate(
            data, dateElement, buyElement,
            sellElement, statusElement, false
        );

    } catch (error) {
        console.error("Error al consultar el tipo de cambio:", error);

        statusElement.textContent =
            "No se pudo actualizar el tipo de cambio.";

        // Mostrar los últimos datos guardados si están disponibles
        const cachedData = localStorage.getItem(cacheKey);

        if (cachedData) {
            try {
                const cache = JSON.parse(cachedData);

                displayExchangeRate(
                    cache.data, dateElement, buyElement,
                    sellElement, statusElement, true
                );

                statusElement.textContent =
                    "Mostrando datos guardados; no se pudo actualizar.";
            } catch {
                // La caché no contiene datos válidos.
            }
        }
    }
}

function displayExchangeRate(
    data, dateElement, buyElement,
    sellElement, statusElement, fromCache
) {
    const dollar = data.dolar ?? data.Dolar;

    if (!dollar?.compra || !dollar?.venta) {
        throw new Error("La respuesta no contiene los datos esperados.");
    }

    buyElement.textContent = Number(dollar.compra.valor)
        .toLocaleString("es-CR", {
            style: "currency",
            currency: "CRC"
        });

    sellElement.textContent = Number(dollar.venta.valor)
        .toLocaleString("es-CR", {
            style: "currency",
            currency: "CRC"
        });

    dateElement.textContent = new Date()
        .toLocaleDateString("es-CR");

    statusElement.textContent = fromCache
        ? "Datos guardados en caché"
        : "Datos actualizados";
}

// Ejecutar la consulta cuando se carga la página
document.addEventListener("DOMContentLoaded", loadExchangeRate);
