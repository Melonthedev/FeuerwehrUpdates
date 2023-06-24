if (isPushNotificationSupported()) 
  registerServiceWorker();

document.querySelector("#demo").addEventListener("click", () => {
  askForPermission(() => sendNotification());
});
  
navigator.serviceWorker.ready.then(worker => {
document.querySelector("#click").addEventListener("click", () => {
    //worker.active.postMessage(loggingdata); 
    navigator.serviceWorker.controller.postMessage({'cmd': 'subscribe'});
    //worker.controller.postMessage({'cmd': 'subscribe'});
  });
});

function askForPermission(granted) {
  initializePushNotifications().then((consent) => {
    if(consent === 'granted') {
      granted();
    } else {
      const h3 = document.getElementById("information");
      h3.innerText = "Notification Permission is required for Fewerwehrupdates to work!";
    }
  });
}

