self.addEventListener("notificationclick", (event) => {
    console.log("Hello clicked Notification.");
    console.log(event.notification.data.operationid);
    console.log(event.notification.data.articlelink + " lolol");
    console.log(event.action);
    
    if (event.notification.action === "showmore") {
        event.waitUntil(clients.openWindow("/?operationid=" + event.notification.data.operationid));
        event.notification.close();
    } else if (event.action === "openarticle") {
        event.waitUntil(clients.openWindow(event.notification.data.articlelink));
    }
});

// urlB64ToUint8Array is a magic function that will encode the base64 public key
// to Array buffer which is needed by the subscription option
const urlB64ToUint8Array = base64String => {
  const padding = '='.repeat((4 - (base64String.length % 4)) % 4)
  const base64 = (base64String + padding).replace(/\-/g, '+').replace(/_/g, '/')
  const rawData = atob(base64)
  const outputArray = new Uint8Array(rawData.length)
  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i)
  }
  return outputArray
}

const saveSubscription = async subscription => {
  var payloadObject = {
    title: "TITLEW",
    message: "message"
};
  //const SERVER_URL = 'https://feuerwehrupdates.melonthedev.wtf/api/Subscription/Save'
  const SERVER_URL = 'https://localhost:7047/api/Subscription/Save'
  const response = await fetch(SERVER_URL, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(subscription)
  })
  return response;
}

self.addEventListener('activate', async () => {
  try {
    const applicationServerKey = urlB64ToUint8Array('BH-9RrroeoEqN37m3SHxOVU97dSOEud8mrkyFAp-O8clW3zNdHjvfwfZ6vwkiR61gob7UqQALHOEoG57qTaK6B4')
    const options = { applicationServerKey, userVisibleOnly: true }
    const subscription = await self.registration.pushManager.subscribe(options)
    console.log(JSON.stringify(subscription))
    console.log(subscription)
    const response = await saveSubscription(subscription)
    console.log(response)
  } catch (err) {
    console.log('Error', err)
  }
});

self.addEventListener('push', (event) => {
  if (!event.data) return;
  const data = event.data.json();
  event.waitUntil(sendNotification(data.title, data.content, data.tag, data.presslink, "123"));
});

function sendNotification(title, content, tag, presslink, id) {
  const options = {
    body: content,
    icon: "./image.png",
    badge: "./badge.png",
    tag: tag,
    actions: [{ action: "showmore", title: "Mehr infos" }, { action: "openarticle", title: "Presseartikel" }],
    data: { "operationid" : "123", "articlelink" : presslink }
  }
  this.registration.showNotification(title, options);
}




/*  const text = "02:28 Uhr - 20 - B\nEinsatzort: Dietershan\nFahrzeuge: 46, 19\nDauer: 15.04.2023 02:28 Uhr - 03:15 Uhr\nSchleifen: 2, Kleinalarm";
  const titleold = "Einsatz: F2 Zimmerbrand";
  const optionsold = {
    body: text,
    icon: "./image.png",
    vibrate: [200, 100, 200],
    tag: "neuer-einsatz",
    //image: img,
    badge: "./badge.png",
    actions: [{ action: "showmore", title: "Mehr infos" }, { action: "openarticle", title: "Presseartikel" }],
    data: { "operationid" : "123", "articlelink" : "https://osthessen-news.de/" }
  };
*/