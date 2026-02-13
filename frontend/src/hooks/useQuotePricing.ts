import { useState, useCallback } from 'react'

export interface QuotePricingResult {
  totalCost: number
  totalPrice: number
  margin: number
  marginStatus: 'green' | 'amber' | 'red'
  vat: number
  priceBeforeVat: number
}

export interface LineItemPricing {
  lineTotal: number
  unitPrice: number
}

export function useQuotePricing(
  defaultMarkup = 0.5,
  defaultVatRate = 0.2,
  greenThreshold = 0.4,
  amberThreshold = 0.2
) {
  const [markup, setMarkup] = useState(defaultMarkup)
  const [vatRate, setVatRate] = useState(defaultVatRate)

  const calculateLineTotal = useCallback(
    (unitCost: number, quantity: number): LineItemPricing => {
      if (unitCost < 0) throw new Error('Unit cost cannot be negative')
      if (quantity <= 0) throw new Error('Quantity must be greater than 0')
      if (markup < 0) throw new Error('Markup cannot be negative')

      const lineTotal = unitCost * quantity * (1 + markup)
      const unitPrice = unitCost * (1 + markup)

      return { lineTotal, unitPrice }
    },
    [markup]
  )

  const calculateQuotePricing = useCallback(
    (totalCost: number): QuotePricingResult => {
      if (totalCost < 0) throw new Error('Total cost cannot be negative')
      if (markup < 0) throw new Error('Markup cannot be negative')
      if (vatRate < 0 || vatRate > 1) throw new Error('VAT rate must be between 0 and 1')

      const priceBeforeVat = totalCost * (1 + markup)
      const vat = priceBeforeVat * vatRate
      const totalPrice = priceBeforeVat + vat

      let margin = 0
      if (totalPrice > 0) {
        margin = (totalPrice - totalCost) / totalPrice
      }

      let marginStatus: 'green' | 'amber' | 'red' = 'red'
      if (margin >= greenThreshold) {
        marginStatus = 'green'
      } else if (margin >= amberThreshold) {
        marginStatus = 'amber'
      }

      return {
        totalCost,
        totalPrice: Math.round(totalPrice * 100) / 100,
        margin: Math.round(margin * 10000) / 10000,
        marginStatus,
        vat: Math.round(vat * 100) / 100,
        priceBeforeVat: Math.round(priceBeforeVat * 100) / 100,
      }
    },
    [markup, vatRate, greenThreshold, amberThreshold]
  )

  const calculateLineItemsTotal = useCallback(
    (lineItems: Array<{ unitCost: number; quantity: number }>): number => {
      return lineItems.reduce((sum, item) => {
        const { lineTotal } = calculateLineTotal(item.unitCost, item.quantity)
        return sum + lineTotal
      }, 0)
    },
    [calculateLineTotal]
  )

  const formatCurrency = useCallback((value: number): string => {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP',
    }).format(value)
  }, [])

  const formatPercentage = useCallback((value: number): string => {
    return `${(value * 100).toFixed(1)}%`
  }, [])

  return {
    markup,
    setMarkup,
    vatRate,
    setVatRate,
    greenThreshold,
    amberThreshold,
    calculateLineTotal,
    calculateQuotePricing,
    calculateLineItemsTotal,
    formatCurrency,
    formatPercentage,
  }
}
