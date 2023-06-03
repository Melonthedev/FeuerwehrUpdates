navigator.serviceWorker.register("sw.js");
Notification.requestPermission((result) => {
  if (result === "granted") {
    navigator.serviceWorker.ready.then((registration) => {
      // Show a notification that includes an action titled Archive.
      registration.showNotification("New mail from Alice", {
        actions: [
          {
            action: "archive",
            title: "Archive",
          },
        ],
      });
    });
  }
});

self.addEventListener(
  "notificationclick",
  (event) => {
    event.notification.close();
    if (event.action === "archive") {
      // User selected the Archive action.
      archiveEmail();
    } else {
      // User selected (e.g., clicked in) the main body of notification.
      alert("hii")
      
      clients.openWindow("/inbox");
    }
  },
  false
);
