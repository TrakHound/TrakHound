(() => {
    const maximumRetryCount = 30000;
    const retryIntervalMilliseconds = 5000;
    const reconnectionPanel = document.getElementById('reconnection-panel');
    const reconnectionToastLabel = document.getElementById('reconnection-panel-label');

    const startReconnectionProcess = () => {
        reconnectionPanel.style.display = 'block';

        let isCanceled = false;

        (async () => {
            for (let i = 0; i < maximumRetryCount; i++) {
                reconnectionToastLabel.innerText = `Retry in ${retryIntervalMilliseconds / 1000}s`;

                await new Promise(resolve => setTimeout(resolve, retryIntervalMilliseconds));

                if (isCanceled) {
                    return;
                }

                try {
                    const result = await Blazor.reconnect();
                    if (!result) {
                        // The server was reached, but the connection was rejected; reload the page.
                        location.reload();
                        return;
                    }

                    // Successfully reconnected to the server.
                    return;
                } catch {
                    // Didn't reach the server; try again.
                }
            }

            // Retried too many times; reload the page.
            location.reload();
        })();

        return {
            cancel: () => {
                isCanceled = true;
                reconnectionPanel.style.display = 'none';
            },
        };
    };

    let currentReconnectionProcess = null;

    Blazor.start({
        circuit: {
            configureSignalR: function (builder) {
                let c = builder.build();
                c.serverTimeoutInMilliseconds = 30000;
                c.keepAliveIntervalInMilliseconds = 5000;
                builder.build = () => {
                    return c;
                };
            },
            reconnectionHandler: {
                onConnectionDown: () => currentReconnectionProcess ??= startReconnectionProcess(),
                onConnectionUp: () => {
                    currentReconnectionProcess?.cancel();
                    currentReconnectionProcess = null;
                }
            }
        }
    });
})();