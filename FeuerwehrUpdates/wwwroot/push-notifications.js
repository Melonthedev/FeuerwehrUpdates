
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
  const text = "20-B: 18:37 Uhr\nEinsatzort: Mond\nFahrzeuge: 46, 19\nDauer: " + new Date().toLocaleDateString() + " 18:37 Uhr - 19:15 Uhr\nSchleifen: 2, Kleinalarm";
  const title = "Einsatz: F2 Fahrzeugbrand";
  const options = {
    body: text,
    icon: "./image.png",
    tag: "neuer-einsatz",
    badge: "./badge.png",
    actions: [{ action: "showmore", title: "Mehr infos" }, { action: "openarticle", title: "Presseartikel" }],
    data: { "operationid" : "demo", "articlelink" : "https://youtu.be/dQw4w9WgXcQ" }
  };
  navigator.serviceWorker.ready.then((serviceWorker) => {
    serviceWorker.showNotification(title, options);
  });
}


/*
.then((registration) => {
    registration.addEventListener('activate', async () => {
      confirm("DEBUG")
      console.debug("[ServiceWorker] Activated!")
    });
  });*/