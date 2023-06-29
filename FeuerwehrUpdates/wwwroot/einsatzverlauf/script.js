loadEinsatzVerlauf();


function loadEinsatzVerlauf() {
    fetch("/api/einsatzverlauf/", {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        }
    })
    .then(response => response.json())
    .then(json => {
        console.log(json);
        if (json.status == 400) 
            return;
        Array.forEach
        json.forEach(entry => {
            createEntry(entry);
        });
    });
}


function createEntry(entry) {
    var container = document.createElement('div');
    var title = document.createElement('h2');
    var documentName = document.createElement('h3');
    var location = document.createElement('h3');
    container.classList.add("einsatzentry");
    title.innerText = entry.einsatzInfo;
    container.appendChild(title);
    documentName.innerText = entry.documentName;
    container.appendChild(documentName);
    location.innerText = entry.location;
    container.appendChild(location);
    container.onclick = () => document.location = "/einsatz/?id=" + entry.id;
    document.querySelector("#entries").prepend(container);
} 