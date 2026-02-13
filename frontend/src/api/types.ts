/**
 * API response types for the Catering Quotes application.
 */

export interface Customer {
  id: number
  name: string
  email: string
  phone?: string
  company?: string
  createdAt: string
  modifiedAt?: string
}

export interface FoodItem {
  id: number
  name: string
  description: string
  costPrice: number
  allergens?: number
  dietaryTags?: number
  isActive: boolean
  createdAt: string
  modifiedAt?: string
}

export interface Allergen {
  id: number
  code: string
  name: string
  description: string
  isActive: boolean
}

export interface DietaryTag {
  id: number
  code: string
  name: string
  description: string
  isActive: boolean
}

export interface QuoteLineItem {
  id: number
  quoteId: number
  foodItemId: number
  description: string
  quantity: number
  unitCost: number
  unitPrice: number
  lineTotal: number
  displayOrder: number
}

export interface Quote {
  id: number
  customerId: number
  quoteNumber: string
  quoteDate: string
  eventDate?: string
  status: string
  vatRate: number
  totalCost: number
  totalPrice: number
  margin: number
  markupPercentage: number
  notes?: string
  createdAt: string
  modifiedAt?: string
  sentAt?: string
  customer?: Customer
  lineItems?: QuoteLineItem[]
}

export interface AppSettings {
  id: number
  defaultVatRate: number
  defaultMarkupPercentage: number
  marginGreenThresholdPct: number
  marginAmberThresholdPct: number
  createdAt: string
  modifiedAt?: string
}

export interface ApiError {
  message: string
  errors?: Record<string, string[]>
}
