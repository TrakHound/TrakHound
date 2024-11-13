# MessageQueues System HTTP API
MessageQueues is a System Level API in TrakHound used to send/receive messages in a way that mimics an AMQP Direct Queue.

## Subscribe
The Subscribe endpoint is served using a WebSockets connection. The Queue is passed in the URL. Messages are received in the format below:

### Request

**Subscribe to 'q-1234'**
```
ws://localhost:8472/_message-queues/subscribe?queue=q-1234
```

**Subscribe to 'q-1234' and don't Auto-Acknowledge**
```
ws://localhost:8472/_message-queues/subscribe?queue=q-1234&acknowledge=false
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>queue</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Queue to subscribe to</td>
        </tr>  
        <tr>
            <td>acknowledge</td>
            <td>Boolean</td>
            <td>NO</td>
            <td>A flag to determine whether to Auto-Acknowledge (true) or not (false). Default = true</td>
        </tr>    
    </tbody>
</table>

### Response
The Message Response uses a simple Headers & Body format as shown below:
```
[DELIVERY_ID]\r
[TIMESTAMP]\r
\r\n
[CONTENT_BYTES]
```
<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Property</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>DELIVERY_ID</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID to use when Acknowledging or Rejecting the message manually</td>
        </tr>  
        <tr>
            <td>TIMESTAMP</td>
            <td>Integer (64)</td>
            <td>YES</td>
            <td>The timestamp (in UNIX nanoseconds) associated with the message</td>
        </tr>    
        <tr>
            <td>CONTENT_BYTES</td>
            <td>Byte[]</td>
            <td>YES</td>
            <td>The content of the message</td>
        </tr> 
    </tbody>
</table>

#### Example (text/plain)
```
q-1234
1731499294317270500

My Name is John Doe
```

#### Example (application/json)
```
q-1234
1731499294317270500

{"FirstName": "John", "LastName": "Doe"}
```
