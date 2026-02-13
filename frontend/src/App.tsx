import { useEffect, useState } from 'react'
import './App.css'

interface ApiStatus {
  message: string
  connected: boolean
}

function App() {
  const [apiStatus, setApiStatus] = useState<ApiStatus>({
    message: 'Checking API...',
    connected: false,
  })
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const checkApi = async () => {
      try {
        const response = await fetch(`${import.meta.env.VITE_API_URL}/`)
        const data = await response.json()
        setApiStatus({
          message: data.message,
          connected: true,
        })
      } catch (error) {
        setApiStatus({
          message: `API Error: ${error instanceof Error ? error.message : 'Unknown error'}`,
          connected: false,
        })
      } finally {
        setLoading(false)
      }
    }

    checkApi()
  }, [])

  return (
    <div className="app">
      <header className="app-header">
        <h1>Catering Quotes</h1>
        <p>Manage your catering quotes efficiently</p>
      </header>

      <main className="app-main">
        <section className="status-section">
          <h2>API Status</h2>
          <div className={`status ${apiStatus.connected ? 'connected' : 'disconnected'}`}>
            {loading ? (
              <p>Checking API connection...</p>
            ) : (
              <>
                <p className="status-message">{apiStatus.message}</p>
                <p className="status-indicator">
                  {apiStatus.connected ? '✓ Connected' : '✗ Disconnected'}
                </p>
              </>
            )}
          </div>
        </section>

        <section className="info-section">
          <h2>Getting Started</h2>
          <ul>
            <li>View existing quotes</li>
            <li>Create new quotes</li>
            <li>Manage food items</li>
            <li>Calculate pricing</li>
          </ul>
        </section>

        <section className="env-section">
          <h2>Environment</h2>
          <dl>
            <dt>Environment:</dt>
            <dd>{import.meta.env.VITE_APP_ENV}</dd>
            <dt>API URL:</dt>
            <dd>{import.meta.env.VITE_API_URL}</dd>
          </dl>
        </section>
      </main>
    </div>
  )
}

export default App
