import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useQuotes } from '../hooks'
import '../styles/QuotesList.css'

export function QuotesList() {
  const { quotes, loading, error, pagination, fetchQuotes, deleteQuote } = useQuotes()
  const [statusFilter, setStatusFilter] = useState<string>('')

  useEffect(() => {
    fetchQuotes(pagination.page, statusFilter || undefined)
  }, [pagination.page, statusFilter])

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this quote?')) {
      await deleteQuote(id)
    }
  }

  return (
    <div className="quotes-list">
      <header className="quotes-header">
        <h1>Quotes</h1>
        <Link to="/quotes/new" className="btn primary">
          + New Quote
        </Link>
      </header>

      {error && <div className="error-message">{error.message}</div>}

      <div className="filters">
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)} className="filter-select">
          <option value="">All Statuses</option>
          <option value="Draft">Draft</option>
          <option value="Sent">Sent</option>
          <option value="Accepted">Accepted</option>
          <option value="Rejected">Rejected</option>
        </select>
      </div>

      {loading ? (
        <p>Loading quotes...</p>
      ) : quotes.length === 0 ? (
        <p>No quotes found.</p>
      ) : (
        <>
          <table className="quotes-table">
            <thead>
              <tr>
                <th>Quote Number</th>
                <th>Customer</th>
                <th>Status</th>
                <th>Total Cost</th>
                <th>Total Price</th>
                <th>Margin</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {quotes.map((quote) => (
                <tr key={quote.id}>
                  <td>{quote.quoteNumber}</td>
                  <td>{quote.customer?.name || 'N/A'}</td>
                  <td>
                    <span className={`status-badge ${quote.status.toLowerCase()}`}>{quote.status}</span>
                  </td>
                  <td>£{quote.totalCost.toFixed(2)}</td>
                  <td>£{quote.totalPrice.toFixed(2)}</td>
                  <td>
                    <span className={`margin-badge ${getMarginStatus(quote.margin)}`}>
                      {(quote.margin * 100).toFixed(1)}%
                    </span>
                  </td>
                  <td className="actions">
                    <Link to={`/quotes/${quote.id}`} className="link-btn">
                      View
                    </Link>
                    {quote.status === 'Draft' && (
                      <>
                        <Link to={`/quotes/${quote.id}/edit`} className="link-btn">
                          Edit
                        </Link>
                        <button onClick={() => handleDelete(quote.id)} className="link-btn delete">
                          Delete
                        </button>
                      </>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="pagination">
            <button
              onClick={() => pagination.goToPreviousPage()}
              disabled={!pagination.hasPreviousPage}
              className="btn-secondary"
            >
              Previous
            </button>
            <span>
              Page {pagination.page} of {pagination.totalPages}
            </span>
            <button
              onClick={() => pagination.goToNextPage()}
              disabled={!pagination.hasNextPage}
              className="btn-secondary"
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  )
}

function getMarginStatus(margin: number): string {
  if (margin >= 0.4) return 'green'
  if (margin >= 0.2) return 'amber'
  return 'red'
}
