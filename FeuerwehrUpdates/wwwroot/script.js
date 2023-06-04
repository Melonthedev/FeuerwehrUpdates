if (isPushNotificationSupported()) 
  registerServiceWorker();

document.querySelector("#demo").addEventListener("click", () => {
  askForPermission(() => sendNotification());
});
  
document.querySelector("#click").addEventListener("click", () => {
  navigator.serviceWorker.ready.then(worker => {
    //worker.active.postMessage(loggingdata); 
    navigator.serviceWorker.controller.postMessage({'cmd': 'subscribe'});
  });
});

function askForPermission(granted) {
  initializePushNotifications().then((consent) => {
    if(consent === 'granted') {
      granted();
    } else {
      const h3 = document.createElement("h3");
      h3.innerText = "Notification Permission is required for Fewerwehrupdates to work!";
      document.body.appendChild(h3);
    }
  });
}

