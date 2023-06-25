loadEinsatzData();

const title = document.querySelector("#title");
const documentName = document.querySelector("#documentName");
const dateAndTime = document.querySelector("#dateAndTime");
const locationText = document.querySelector("#location");
const vehicles = document.querySelector("#vehicles");
const schleifen = document.querySelector("#schleifen");
const presslink = document.querySelector("#presslink");
const source = document.querySelector("#source");
const idInfo = document.querySelector("#idInfo");
const info = document.getElementById("information");


function loadEinsatzData() {
    const queryString = window.location.search;
    const parameters = new URLSearchParams(queryString);
    const id = parameters.get('id');
    console.log(id);
    if (id == "demo") return;
    fetch("/api/einsatzverlauf/" + id, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        }
    })
    .then(response => response.json())
    .then(json => {
        console.log(json);
        if (json.status == 400) {
            title.innerText = "Einsatz wurde nicht gefunden";
            documentName.innerText = "Der Einsatz mit der ID " + id + " wurde nicht gefunden";
            dateAndTime.innerText = "";
            locationText.innerText = "Ort: 404 :)";
            vehicles.innerText = "";
            schleifen.innerText = "";
            idInfo.innerText = "";
            return;
        }
        title.innerText = json.einsatzInfo;
        documentName.innerText = json.documentName;
        dateAndTime.innerText = "Am " + json.date + " von " + json.startedTime + " bis " + json.endTime;
        locationText.innerText = "Ort: " + json.location;
        vehicles.innerText =  "Fahrzeuge: " + json.vehicles;
        schleifen.innerText = json.einsatzSchleifen == null ? "" : "Schleifen: " + json.einsatzSchleifen;
        idInfo.innerHTML = "Einsatz Id: " + json.einsatzId + "<br><span style='font-size: 12px'>" + json.id + "</span>";
        presslink.onclick = () => {
            if (json.pressLink == null) {
                info.innerText = "Kein Presselink hinterlegt"
                return;
            }
            document.location = json.pressLink;
        };
        source.onclick = () => {
            if (json.documentUrl == null) {
                info.innerText = "Keine Quelle hinterlegt"
                return;
            }
            document.location = json.documentUrl;
        };
    });
}