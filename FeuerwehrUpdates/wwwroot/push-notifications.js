
function isPushNotificationSupported() {
  return "serviceWorker" in navigator && "PushManager" in window;
}

function initializePushNotifications() {
  return Notification.requestPermission((result) =>  { return result; });
}

function registerServiceWorker() {
  navigator.serviceWorker.register("./sw.js");
}

function sendNotification() {
  const text = "02:28 Uhr - 20 - B\nEinsatzort: Dietershan\nFahrzeuge: 46, 19\nDauer: 15.04.2023 02:28 Uhr - 03:15 Uhr\nSchleifen: 2, Kleinalarm";
  const title = "Einsatz: F2 Zimmerbrand";
  const options = {
    body: text,
    icon: "./image.png",
    vibrate: [200, 100, 200],
    tag: "neuer-einsatz",
    //image: img,
    badge: "./badge.png",
    actions: [{ action: "showmore", title: "Mehr infos" }, { action: "openarticle", title: "Presseartikel" }],
    data: { "operationid" : "123", "articlelink" : "https://osthessen-news.de/" }
  };
  navigator.serviceWorker.ready.then((serviceWorker) => {
    serviceWorker.showNotification(title, options);
  });
}