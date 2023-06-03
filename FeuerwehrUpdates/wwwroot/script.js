//alert("penis")
if (isPushNotificationSupported()) {
    registerServiceWorker();
    initializePushNotifications().then((consent) => {
      if(consent === 'granted') {
        document.querySelector("#click").addEventListener("click", () => sendNotification());
      } else {
        const h3 = document.createElement("h3");
        h3.innerText = "Notification Permission is required for Fewerwehrupdates to work!";
        document.body.appendChild(h3);
      }
    });
}