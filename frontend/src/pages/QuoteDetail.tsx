import { useEffect, useState } from 'react'
import { useParams, Link, useNavigate } from 'react-router-dom'
import { Quote } from '../api/types'
import { useQuotes } from '../hooks'
import '../styles/QuoteDetail.css'

export function QuoteDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { loading, error, getQuote, deleteQuote } = useQuotes()
  const [quote, setQuote] = useState<Quote | null>(null)

  useEffect(() => {
    const loadQuote = async () => {
      if (id) {
        const loadedQuote = await getQuote(parseInt(id, 10))
        setQuote(loadedQuote)
      }
    }
    loadQuote()
  }, [id])

  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this quote?')) {
      if (id && (await deleteQuote(parseInt(id, 10)))) {
        navigate('/quotes')
      }
    }
  }

  if (loading) return <p>Loading quote...</p>
  if (error) return <div className="error-message">{error.message}</div>
  if (!quote) return <p>Quote not found.</p>

  const marginStatus = quote.margin >= 0.4 ? 'green' : quote.margin >= 0.2 ? 'amber' : 'red'

  return (
    <div className="quote-detail">
      <header className="quote-header">
        <div>
          <h1>{quote.quoteNumber}</h1>
          <p>{quote.customer?.name}</p>
        </div>
        <div className="quote-status">
          <span className={`status-badge ${quote.status.toLowerCase()}`}>{quote.status}</span>
        </div>
      </header>

      <main className="quote-content">
        <section className="quote-info">
          <h2>Quote Details</h2>
          <dl className="info-list">
            <dt>Status:</dt>
            <dd>{quote.status}</dd>
            <dt>Quote Date:</dt>
            <dd>{new Date(quote.quoteDate).toLocaleDateString()}</dd>
            {quote.eventDate && (
              <>
                <dt>Event Date:</dt>
                <dd>{new Date(quote.eventDate).toLocaleDateString()}</dd>
              </>
            )}
            <dt>VAT Rate:</dt>
            <dd>{(quote.vatRate * 100).toFixed(1)}%</dd>
            <dt>Markup:</dt>
            <dd>{(quote.markupPercentage * 100).toFixed(1)}%</dd>
          </dl>
        </section>

        <section className="line-items">
          <h2>Line Items</h2>
          {quote.lineItems && quote.lineItems.length > 0 ? (
            <table className="items-table">
              <thead>
                <tr>
                  <th>Description</th>
                  <th>Quantity</th>
                  <th>Unit Cost</th>
                  <th>Line Total</th>
                </tr>
              </thead>
              <tbody>
                {quote.lineItems.map((item) => (
                  <tr key={item.id}>
                    <td>{item.description}</td>
                    <td>{item.quantity}</td>
                    <td>£{item.unitCost.toFixed(2)}</td>
                    <td>£{item.lineTotal.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p>No line items.</p>
          )}
        </section>

        <section className="pricing-summary">
          <h2>Pricing</h2>
          <dl className="summary-list">
            <dt>Total Cost:</dt>
            <dd>£{quote.totalCost.toFixed(2)}</dd>
            <dt>Price (before VAT):</dt>
            <dd>£{(quote.totalPrice / (1 + quote.vatRate)).toFixed(2)}</dd>
            <dt>VAT ({(quote.vatRate * 100).toFixed(1)}%):</dt>
            <dd>£{(quote.totalPrice - quote.totalPrice / (1 + quote.vatRate)).toFixed(2)}</dd>
            <dt>Total Price:</dt>
            <dd className="total-price">£{quote.totalPrice.toFixed(2)}</dd>
            <dt>Margin:</dt>
            <dd>
              <span className={`margin-badge ${marginStatus}`}>{(quote.margin * 100).toFixed(1)}%</span>
            </dd>
          </dl>
        </section>

        {quote.notes && (
          <section className="notes">
            <h2>Notes</h2>
            <p>{quote.notes}</p>
          </section>
        )}

        <section className="quote-actions">
          <div className="actions">
            {quote.status === 'Draft' && (
              <>
                <Link to={`/quotes/${quote.id}/edit`} className="btn primary">
                  Edit Quote
                </Link>
                <button onClick={handleDelete} className="btn danger">
                  Delete Quote
                </button>
              </>
            )}
            <Link to="/quotes" className="btn secondary">
              Back to Quotes
            </Link>
          </div>
        </section>
      </main>
    </div>
  )
}
