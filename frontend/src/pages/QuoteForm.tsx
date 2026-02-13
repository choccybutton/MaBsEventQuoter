import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { Quote } from '../api/types'
import { useQuotes, useCustomers, useFoodItems, useQuotePricing, useValidation } from '../hooks'
import '../styles/QuoteForm.css'

interface FormLineItem {
  id?: number
  foodItemId?: number
  description: string
  quantity: number
  unitCost: number
  unitPrice: number
  lineTotal: number
  displayOrder: number
}

export function QuoteForm() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const isEditMode = !!id

  const { getQuote, createQuote, updateQuote } = useQuotes()
  const { customers, fetchCustomers } = useCustomers()
  const { foodItems, fetchFoodItems } = useFoodItems()
  const { calculateLineTotal, calculateQuotePricing, formatCurrency } = useQuotePricing(0.5, 0.2)
  const { errors, validateForm } = useValidation()

  const [loading, setLoading] = useState(isEditMode)
  const [submitting, setSubmitting] = useState(false)
  const [quote, setQuote] = useState<Quote | null>(null)

  const [customerId, setCustomerId] = useState<number>(0)
  const [eventDate, setEventDate] = useState<string>('')
  const [vatRate, setVatRate] = useState<number>(0.2)
  const [markupPercentage, setMarkupPercentage] = useState<number>(0.5)
  const [notes, setNotes] = useState<string>('')
  const [lineItems, setLineItems] = useState<FormLineItem[]>([])

  const [pricing, setPricing] = useState({
    totalCost: 0,
    totalPrice: 0,
    vat: 0,
    margin: 0,
    marginStatus: 'green' as 'green' | 'amber' | 'red',
  })

  // Load initial data
  useEffect(() => {
    fetchCustomers(1)
    fetchFoodItems(1)
  }, [])

  // Load quote for edit mode
  useEffect(() => {
    const loadQuote = async () => {
      if (isEditMode && id) {
        const loaded = await getQuote(parseInt(id, 10))
        if (loaded) {
          setQuote(loaded)
          setCustomerId(loaded.customerId)
          setEventDate(loaded.eventDate || '')
          setVatRate(loaded.vatRate)
          setMarkupPercentage(loaded.markupPercentage)
          setNotes(loaded.notes || '')
          if (loaded.lineItems) {
            setLineItems(
              loaded.lineItems.map((item) => ({
                id: item.id,
                foodItemId: item.foodItemId,
                description: item.description,
                quantity: item.quantity,
                unitCost: item.unitCost,
                unitPrice: item.unitPrice,
                lineTotal: item.lineTotal,
                displayOrder: item.displayOrder,
              }))
            )
          }
        }
        setLoading(false)
      }
    }
    loadQuote()
  }, [id, isEditMode])

  // Recalculate pricing when line items or rates change
  useEffect(() => {
    if (lineItems.length === 0) {
      setPricing({
        totalCost: 0,
        totalPrice: 0,
        vat: 0,
        margin: 0,
        marginStatus: 'green',
      })
      return
    }

    const totalCost = lineItems.reduce((sum, item) => sum + item.lineTotal / (1 + markupPercentage), 0)
    const result = calculateQuotePricing(totalCost)
    setPricing({
      totalCost: result.totalCost,
      totalPrice: result.totalPrice,
      vat: result.vat,
      margin: result.margin,
      marginStatus: result.marginStatus,
    })
  }, [lineItems, markupPercentage, vatRate, calculateQuotePricing])

  const handleAddLineItem = (foodItemId?: number) => {
    const foodItem = foodItems.find((f) => f.id === foodItemId)
    const newItem: FormLineItem = {
      foodItemId,
      description: foodItem?.name || '',
      quantity: 1,
      unitCost: foodItem?.costPrice || 0,
      unitPrice: 0,
      lineTotal: 0,
      displayOrder: lineItems.length,
    }

    const { lineTotal, unitPrice } = calculateLineTotal(newItem.unitCost, newItem.quantity)
    newItem.lineTotal = lineTotal
    newItem.unitPrice = unitPrice

    setLineItems([...lineItems, newItem])
  }

  const handleUpdateLineItem = (index: number, field: string, value: unknown) => {
    const updatedItems = [...lineItems]
    const item = updatedItems[index]

    if (field === 'quantity' || field === 'unitCost') {
      if (field === 'quantity') {
        item.quantity = value as number
      } else {
        item.unitCost = value as number
      }
      const { lineTotal, unitPrice } = calculateLineTotal(item.unitCost, item.quantity)
      item.lineTotal = lineTotal
      item.unitPrice = unitPrice
    } else if (field === 'description') {
      item.description = value as string
    }

    setLineItems(updatedItems)
  }

  const handleRemoveLineItem = (index: number) => {
    setLineItems(lineItems.filter((_, i) => i !== index))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    // Validate form
    const isValid = validateForm(
      { customerId, eventDate, vatRate, markupPercentage },
      {
        customerId: [{ type: 'positive', message: 'Please select a customer' }],
        vatRate: [{ type: 'percentage' }],
        markupPercentage: [{ type: 'percentage' }],
      }
    )

    if (!isValid || lineItems.length === 0) {
      if (lineItems.length === 0) {
        alert('Please add at least one line item')
      }
      return
    }

    setSubmitting(true)

    try {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const quoteData: any = {
        customerId,
        eventDate: eventDate || new Date().toISOString().split('T')[0],
        vatRate,
        markupPercentage,
        notes,
        lineItems: lineItems.map((item, index) => ({
          foodItemId: item.foodItemId || 0,
          description: item.description,
          quantity: item.quantity,
          unitCost: item.unitCost,
          displayOrder: index,
        })),
      }

      if (isEditMode && id) {
        await updateQuote(parseInt(id, 10), quoteData)
        navigate(`/quotes/${id}`)
      } else {
        const newQuote = await createQuote(quoteData)
        if (newQuote) {
          navigate(`/quotes/${newQuote.id}`)
        }
      }
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) return <p>Loading quote...</p>

  return (
    <div className="quote-form">
      <header className="form-header">
        <h1>{isEditMode ? 'Edit Quote' : 'Create New Quote'}</h1>
        <p>{isEditMode && quote ? quote.quoteNumber : 'New quote will be auto-numbered'}</p>
      </header>

      <form onSubmit={handleSubmit} className="form-container">
        {/* Customer Section */}
        <section className="form-section">
          <h2>Quote Details</h2>

          <div className="form-group">
            <label htmlFor="customer">Customer *</label>
            <select
              id="customer"
              value={customerId}
              onChange={(e) => setCustomerId(parseInt(e.target.value, 10))}
              className="form-control"
              required
            >
              <option value={0}>Select a customer</option>
              {customers.map((customer) => (
                <option key={customer.id} value={customer.id}>
                  {customer.name} ({customer.email})
                </option>
              ))}
            </select>
            {errors.customerId && <span className="error">{errors.customerId[0]}</span>}
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="eventDate">Event Date</label>
              <input
                id="eventDate"
                type="date"
                value={eventDate}
                onChange={(e) => setEventDate(e.target.value)}
                className="form-control"
              />
            </div>

            <div className="form-group">
              <label htmlFor="vatRate">VAT Rate (%)</label>
              <input
                id="vatRate"
                type="number"
                min="0"
                max="100"
                step="0.1"
                value={vatRate * 100}
                onChange={(e) => setVatRate(parseFloat(e.target.value) / 100)}
                className="form-control"
              />
              {errors.vatRate && <span className="error">{errors.vatRate[0]}</span>}
            </div>

            <div className="form-group">
              <label htmlFor="markup">Markup (%)</label>
              <input
                id="markup"
                type="number"
                min="0"
                step="0.1"
                value={markupPercentage * 100}
                onChange={(e) => setMarkupPercentage(parseFloat(e.target.value) / 100)}
                className="form-control"
              />
              {errors.markupPercentage && <span className="error">{errors.markupPercentage[0]}</span>}
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="notes">Notes</label>
            <textarea
              id="notes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              className="form-control"
              rows={3}
              placeholder="Add any additional notes for this quote"
            />
          </div>
        </section>

        {/* Line Items Section */}
        <section className="form-section">
          <div className="section-header">
            <h2>Line Items</h2>
            <button
              type="button"
              className="btn-add"
              onClick={() => handleAddLineItem()}
            >
              + Add Item
            </button>
          </div>

          {lineItems.length === 0 ? (
            <p className="no-items">No line items yet. Add an item to get started.</p>
          ) : (
            <table className="line-items-table">
              <thead>
                <tr>
                  <th>Description</th>
                  <th>Quantity</th>
                  <th>Unit Cost</th>
                  <th>Unit Price</th>
                  <th>Line Total</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {lineItems.map((item, index) => (
                  <tr key={index}>
                    <td>
                      <input
                        type="text"
                        value={item.description}
                        onChange={(e) => handleUpdateLineItem(index, 'description', e.target.value)}
                        className="input-small"
                      />
                    </td>
                    <td>
                      <input
                        type="number"
                        min="1"
                        value={item.quantity}
                        onChange={(e) => handleUpdateLineItem(index, 'quantity', parseInt(e.target.value, 10))}
                        className="input-small"
                      />
                    </td>
                    <td>
                      <input
                        type="number"
                        min="0"
                        step="0.01"
                        value={item.unitCost}
                        onChange={(e) => handleUpdateLineItem(index, 'unitCost', parseFloat(e.target.value))}
                        className="input-small"
                      />
                    </td>
                    <td className="readonly">{formatCurrency(item.unitPrice)}</td>
                    <td className="readonly font-bold">{formatCurrency(item.lineTotal)}</td>
                    <td>
                      <button
                        type="button"
                        className="btn-remove"
                        onClick={() => handleRemoveLineItem(index)}
                      >
                        Remove
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </section>

        {/* Pricing Summary Section */}
        {lineItems.length > 0 && (
          <section className="form-section pricing-section">
            <h2>Pricing Summary</h2>
            <div className="pricing-grid">
              <div className="pricing-item">
                <span className="label">Total Cost:</span>
                <span className="value">{formatCurrency(pricing.totalCost)}</span>
              </div>
              <div className="pricing-item">
                <span className="label">Price (before VAT):</span>
                <span className="value">{formatCurrency(pricing.totalPrice / (1 + vatRate))}</span>
              </div>
              <div className="pricing-item">
                <span className="label">VAT ({(vatRate * 100).toFixed(1)}%):</span>
                <span className="value">{formatCurrency(pricing.vat)}</span>
              </div>
              <div className="pricing-item total">
                <span className="label">Total Price:</span>
                <span className="value">{formatCurrency(pricing.totalPrice)}</span>
              </div>
              <div className="pricing-item">
                <span className="label">Margin:</span>
                <span className={`value margin-${pricing.marginStatus}`}>
                  {(pricing.margin * 100).toFixed(1)}%
                </span>
              </div>
            </div>
          </section>
        )}

        {/* Form Actions */}
        <section className="form-actions">
          <button type="submit" className="btn primary" disabled={submitting}>
            {submitting ? 'Saving...' : isEditMode ? 'Update Quote' : 'Create Quote'}
          </button>
          <button
            type="button"
            className="btn secondary"
            onClick={() => navigate('/quotes')}
            disabled={submitting}
          >
            Cancel
          </button>
        </section>
      </form>
    </div>
  )
}
