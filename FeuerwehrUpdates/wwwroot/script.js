const button = document.getElementById("click");
const info = document.getElementById("information");


if (isPushNotificationSupported()) 
  registerServiceWorker();
else info.innerText = "Push Benachrichtigungen werden von deinem Browser nicht unterstützt!";

// Redirect to apple installation guide
let platform = navigator?.userAgent || navigator?.platform || 'unknown';
if(/iPhone|iPod|iPad/.test(platform) && !window.matchMedia('(display-mode: standalone)').matches) document.location = "/ios";


document.querySelector("#demo").addEventListener("click", () => {
  askForPermission(() => sendNotification());
});
  
navigator.serviceWorker.ready.then(worker => {
document.querySelector("#click").addEventListener("click", () => {
    button.disabled = true;
    button.style.cursor = "wait";
    askForPermission(() => {
      navigator.serviceWorker.controller.postMessage({'cmd': 'subscribe'});
    });
  });
});

navigator.serviceWorker.addEventListener('message', (e) => {
  console.log(e.data);
  if (!e.data.success) {
    info.innerHTML = "Ein Fehler ist aufgetreten. Aktualisiere die Seite und versuche es erneut.<br>Fehler: " + e.data.error;
    button.disabled = false;
    button.style.cursor = "pointer";
  } else {
    button.disabled = true;
    button.style.backgroundColor = "gray";
    button.style.cursor = "not-allowed";
    info.style.color = "green";
    info.innerHTML = "Super! Das hat geklappt.<br> Du bekommst jetzt Push-Benachrichtigungen bei neuen Einsätzen."
  }
});

function askForPermission(granted) {
  initializePushNotifications().then((consent) => {
    if(consent === 'granted') {
      granted();
    } else {
      info.innerText = "Benachrichtigungen müssen erlaubt sein, damit Feuerwehrupdates richtig funktioniert!";
    }
  });
}

