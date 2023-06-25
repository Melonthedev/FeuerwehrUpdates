self.addEventListener("notificationclick", (event) => {
  event.notification.close();
  console.log(event)
  if (event.action === "openarticle") {
    event.waitUntil(clients.openWindow(event.notification.data.articlelink));
  } else {
    event.waitUntil(clients.openWindow("/einsatz?id=" + event.notification.data.id));
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
  //const SERVER_URL = 'https://feuerwehrupdates.melonthedev.wtf/api/Subscription/Save'
  const SERVER_URL = '/api/Subscription/Save'
  const response = await fetch(SERVER_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(subscription)
  })
  return response;
}

self.addEventListener('activate', async (event) => {
    event.waitUntil(self.clients.claim()); // Become available to all pages
    console.debug("[ServiceWorker] Activated!");
});

self.addEventListener('install', function(event) {
  event.waitUntil(self.skipWaiting()); // Activate worker immediately
});

self.addEventListener('message', async (e) => {
  var response = await createSubscription();
  console.log(response);
  if (e.data.cmd == "subscribe") e.source.postMessage(response);
})

async function createSubscription() {
  try {
    const applicationServerKey = urlB64ToUint8Array('BH-9RrroeoEqN37m3SHxOVU97dSOEud8mrkyFAp-O8clW3zNdHjvfwfZ6vwkiR61gob7UqQALHOEoG57qTaK6B4')
    const options = { applicationServerKey, userVisibleOnly: true }
    const subscription = await self.registration.pushManager.subscribe(options)
    console.log(subscription);
    const response = await saveSubscription(subscription)
    console.log(response);
    return { "success" : true, "error" : null };
  } catch (err) {
    console.error('Error', err);
    return { "success" : false, "error" : err };
  }
}

self.addEventListener('push', (event) => {
  if (!event.data) return;
  const data = event.data.json();
  console.debug(data);
  event.waitUntil(sendNotification(data.Title, data.Content, data.Tag, data.PressLink, data.OperationId, data.Id));
});

function sendNotification(title, content, tag, presslink, operationid, id) {
  let buttons = [{ action: "showmore", title: "Mehr infos" }];
  if (presslink != null && presslink !== "") 
    buttons = [{ action: "showmore", title: "Mehr infos" }, { action: "openarticle", title: "Presseartikel" }];

  const options = {
    body: content,
    icon: "./image.png",
    badge: "./badge.png",
    tag: tag,
    actions: buttons,
    data: { "id" : id, "articlelink" : presslink }
  }
  this.registration.showNotification(title, options);
}