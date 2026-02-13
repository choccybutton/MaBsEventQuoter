import { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useQuotes } from '../hooks'
import '../styles/Dashboard.css'

export function Dashboard() {
  const { quotes, loading, fetchQuotes } = useQuotes()

  useEffect(() => {
    fetchQuotes(1)
  }, [])

  const draftQuotes = quotes.filter((q) => q.status === 'Draft').length
  const sentQuotes = quotes.filter((q) => q.status === 'Sent').length
  const acceptedQuotes = quotes.filter((q) => q.status === 'Accepted').length

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <h1>Dashboard</h1>
        <p>Manage your catering quotes</p>
      </header>

      <main className="dashboard-main">
        <section className="stats-grid">
          <div className="stat-card">
            <h3>Total Quotes</h3>
            <p className="stat-value">{quotes.length}</p>
          </div>
          <div className="stat-card">
            <h3>Draft</h3>
            <p className="stat-value draft">{draftQuotes}</p>
          </div>
          <div className="stat-card">
            <h3>Sent</h3>
            <p className="stat-value sent">{sentQuotes}</p>
          </div>
          <div className="stat-card">
            <h3>Accepted</h3>
            <p className="stat-value accepted">{acceptedQuotes}</p>
          </div>
        </section>

        <section className="quick-actions">
          <h2>Quick Actions</h2>
          <div className="actions-grid">
            <Link to="/quotes/new" className="action-btn primary">
              + New Quote
            </Link>
            <Link to="/quotes" className="action-btn secondary">
              View Quotes
            </Link>
            <Link to="/customers" className="action-btn secondary">
              Manage Customers
            </Link>
            <Link to="/food-items" className="action-btn secondary">
              Manage Food Items
            </Link>
          </div>
        </section>

        {loading ? (
          <p>Loading...</p>
        ) : (
          <section className="recent-quotes">
            <h2>Recent Quotes</h2>
            {quotes.length === 0 ? (
              <p>No quotes yet. Create one to get started!</p>
            ) : (
              <table className="quotes-table">
                <thead>
                  <tr>
                    <th>Quote Number</th>
                    <th>Status</th>
                    <th>Total Price</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {quotes.slice(0, 5).map((quote) => (
                    <tr key={quote.id}>
                      <td>{quote.quoteNumber}</td>
                      <td>
                        <span className={`status-badge ${quote.status.toLowerCase()}`}>{quote.status}</span>
                      </td>
                      <td>£{quote.totalPrice.toFixed(2)}</td>
                      <td>
                        <Link to={`/quotes/${quote.id}`}>View</Link>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </section>
        )}
      </main>
    </div>
  )
}
