# Real-Time ACQUISIZIONI API Documentation

## 🎯 Overview
Your C# API now includes real-time capabilities using **SignalR** and **SQLDependency** to monitor the `ACQUISIZIONI` table and push live updates to connected clients.

## 🚀 Features
- **Real-time monitoring** of the ACQUISIZIONI table
- **Automatic notifications** when data changes (INSERT, UPDATE, DELETE)
- **SignalR Hub** for WebSocket communication
- **RESTful endpoints** for manual data retrieval
- **Background service** that automatically starts with the API

## 📡 API Endpoints

### 1. Get Latest ACQUISIZIONI
```
GET /api/acquisizioni/latest
```
Returns the latest single record from the ACQUISIZIONI table.

### 2. Get Latest Single Record
```
GET /api/acquisizioni/latest-single
```
Returns the most recent single ACQUISIZIONI record.

### 3. Start Monitoring (for testing)
```
POST /api/acquisizioni/start-monitoring
```
Manually start the real-time monitoring.

### 3. Stop Monitoring (for testing)
```
POST /api/acquisizioni/stop-monitoring
```
Manually stop the real-time monitoring.

## 🔌 SignalR Hub
**Hub URL:** `/hubs/acquisizioni`

### Events You Can Listen To:
- `Connected` - Fired when client connects successfully
- `NewAcquisizioneAdded` - Fired when a new record is added (sends single latest record)
- `AcquisizioniUpdated` - Fired when ACQUISIZIONI table changes (sends array with latest record for backward compatibility)

### Methods You Can Call:
- `JoinGroup(groupName)` - Join a specific group
- `LeaveGroup(groupName)` - Leave a specific group

## 📝 Data Models

### AcquisizioneDto
```typescript
interface AcquisizioneDto {
  ID: number;
  COD_LINEA: string;
  FOTO_SUPERIORE?: string;
  FOTO_FRONTALE?: string;
  FOTO_BOX?: string;
  CODICE_ARTICOLO?: string;
  CODICE_ORDINE?: string;
  ABILITA_CQ?: string;
  ESITO_CQ_ARTICOLO?: string;
  SCOSTAMENTO_CQ_ARTICOLO?: string;
  ESITO_CQ_BOX?: string;
  CONFIDENZA_C?: string;
  DT_INS?: string;
  DT_AGG?: string;
  CreatedAt: string; // ISO date string
  UpdatedAt?: string; // ISO date string
}
```

## 🔧 How to Run the API
```bash
cd "c:\WEB\C#\api"
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- SignalR Hub: `https://localhost:5001/hubs/acquisizioni`

## ⚠️ Important Notes
1. **Service Broker** must be enabled on your SQL Server database for SQLDependency to work
2. The API will automatically try to enable Service Broker on startup
3. Real-time monitoring starts automatically when the API starts
4. The connection string should point to your SMARTMOB database

---

# 🌐 React Integration Guide

## Step 1: Install SignalR Client
```bash
npm install @microsoft/signalr
```

## Step 2: Create SignalR Service (React Hook)

Create a file: `hooks/useAcquisizioniRealtime.ts`

```typescript
import { useEffect, useState, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';

interface AcquisizioneDto {
  ID: number;
  COD_LINEA: string;
  FOTO_SUPERIORE?: string;
  FOTO_FRONTALE?: string;
  FOTO_BOX?: string;
  CODICE_ARTICOLO?: string;
  CODICE_ORDINE?: string;
  ABILITA_CQ?: string;
  ESITO_CQ_ARTICOLO?: string;
  SCOSTAMENTO_CQ_ARTICOLO?: string;
  ESITO_CQ_BOX?: string;
  CONFIDENZA_C?: string;
  DT_INS?: string;
  DT_AGG?: string;
  CreatedAt: string;
  UpdatedAt?: string;
}

interface UseAcquisizioniRealtimeReturn {
  acquisizioni: AcquisizioneDto[];
  isConnected: boolean;
  connectionId?: string;
  error?: string;
  reconnect: () => void;
}

export const useAcquisizioniRealtime = (apiUrl: string = 'https://localhost:5001'): UseAcquisizioniRealtimeReturn => {
  const [acquisizioni, setAcquisizioni] = useState<AcquisizioneDto[]>([]);
  const [isConnected, setIsConnected] = useState(false);
  const [connectionId, setConnectionId] = useState<string>();
  const [error, setError] = useState<string>();
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

  const connectToHub = useCallback(async () => {
    try {
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${apiUrl}/hubs/acquisizioni`, {
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect()
        .build();

      // Set up event handlers
      newConnection.on('Connected', (connId: string) => {
        console.log('🔗 Connected to SignalR Hub:', connId);
        setConnectionId(connId);
        setIsConnected(true);
        setError(undefined);
      });

      // Listen for new single records added
      newConnection.on('NewAcquisizioneAdded', (newRecord: AcquisizioneDto) => {
        console.log('🆕 New ACQUISIZIONE added:', newRecord);
        setAcquisizioni(prev => [newRecord, ...prev.slice(0, 49)]); // Keep latest 50, add new one at top
      });

      // Listen for table updates (backward compatibility)
      newConnection.on('AcquisizioniUpdated', (data: AcquisizioneDto[]) => {
        console.log('📊 ACQUISIZIONI updated:', data);
        if (data.length > 0) {
          setAcquisizioni(prev => {
            const newRecord = data[0];
            const exists = prev.some(item => item.ID === newRecord.ID);
            if (!exists) {
              return [newRecord, ...prev.slice(0, 49)];
            }
            return prev;
          });
        }
      });

      newConnection.onclose((error) => {
        console.log('❌ Connection closed:', error);
        setIsConnected(false);
        setConnectionId(undefined);
        if (error) {
          setError(error.message);
        }
      });

      newConnection.onreconnecting((error) => {
        console.log('🔄 Reconnecting...', error);
        setIsConnected(false);
      });

      newConnection.onreconnected((connectionId) => {
        console.log('✅ Reconnected:', connectionId);
        setIsConnected(true);
        setError(undefined);
      });

      await newConnection.start();
      setConnection(newConnection);

      console.log('🚀 SignalR connection started successfully');

    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to connect';
      console.error('❌ SignalR connection error:', errorMessage);
      setError(errorMessage);
      setIsConnected(false);
    }
  }, [apiUrl]);

  const reconnect = useCallback(() => {
    if (connection) {
      connection.stop();
    }
    connectToHub();
  }, [connection, connectToHub]);

  useEffect(() => {
    connectToHub();

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [connectToHub]);

  // Fetch initial data
  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        const response = await fetch(`${apiUrl}/api/acquisizioni/latest-single`);
        if (response.ok) {
          const data = await response.json();
          setAcquisizioni([data]); // Start with the single latest record
        }
      } catch (error) {
        console.error('Failed to fetch initial data:', error);
      }
    };

    if (isConnected) {
      fetchInitialData();
    }
  }, [isConnected, apiUrl]);

  return {
    acquisizioni,
    isConnected,
    connectionId,
    error,
    reconnect,
  };
};
```

## Step 3: Use in Your React Component

```typescript
import React from 'react';
import { useAcquisizioniRealtime } from './hooks/useAcquisizioniRealtime';

const AcquisizioniDashboard: React.FC = () => {
  const { 
    acquisizioni, 
    isConnected, 
    connectionId, 
    error, 
    reconnect 
  } = useAcquisizioniRealtime('https://localhost:5001');

  return (
    <div className="acquisizioni-dashboard">
      <div className="connection-status">
        <h2>📡 Real-Time ACQUISIZIONI Monitor</h2>
        <div style={{ 
          padding: '10px', 
          borderRadius: '5px', 
          backgroundColor: isConnected ? '#d4edda' : '#f8d7da',
          color: isConnected ? '#155724' : '#721c24',
          marginBottom: '20px'
        }}>
          {isConnected ? (
            <>✅ Connected (ID: {connectionId})</>
          ) : (
            <>❌ Disconnected {error && `- ${error}`}</>
          )}
          {!isConnected && (
            <button onClick={reconnect} style={{ marginLeft: '10px' }}>
              🔄 Reconnect
            </button>
          )}
        </div>
      </div>

      <div className="acquisizioni-list">
        <h3>📊 Latest ACQUISIZIONI ({acquisizioni.length} records)</h3>
        <div style={{ maxHeight: '600px', overflowY: 'auto' }}>
          {acquisizioni.map((item) => (
            <div key={item.ID} style={{ 
              border: '1px solid #ddd', 
              margin: '10px 0', 
              padding: '15px',
              borderRadius: '5px',
              backgroundColor: '#f8f9fa'
            }}>
              <div style={{ fontWeight: 'bold', color: '#007bff' }}>
                ID: {item.ID} | Line: {item.COD_LINEA}
              </div>
              <div>Article: {item.CODICE_ARTICOLO || 'N/A'}</div>
              <div>Order: {item.CODICE_ORDINE || 'N/A'}</div>
              <div>Quality Result: {item.ESITO_CQ_ARTICOLO || 'N/A'}</div>
              <div>Box Result: {item.ESITO_CQ_BOX || 'N/A'}</div>
              <div style={{ fontSize: '0.8em', color: '#666' }}>
                Created: {new Date(item.CreatedAt).toLocaleString()}
              </div>
              {item.FOTO_SUPERIORE && (
                <div>
                  📷 Photos: {item.FOTO_SUPERIORE && '📸'} {item.FOTO_FRONTALE && '📸'} {item.FOTO_BOX && '📸'}
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default AcquisizioniDashboard;
```

## Step 4: Add to Your App

```typescript
import AcquisizioniDashboard from './components/AcquisizioniDashboard';

function App() {
  return (
    <div className="App">
      <AcquisizioniDashboard />
    </div>
  );
}

export default App;
```

## 🎉 You're Ready!

1. Start your C# API: `dotnet run`
2. Start your React app: `npm start`
3. Open your React app in the browser
4. Watch as new data automatically appears when ACQUISIZIONI table changes!

## 🧪 Testing the Real-Time Connection

You can test by:
1. Inserting new records directly into the ACQUISIZIONI table via SQL Server Management Studio
2. Using the API endpoints to start/stop monitoring
3. Checking the browser console for SignalR connection logs

The system will automatically detect changes and push them to all connected React clients in real-time! 🚀
