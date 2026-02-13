import client from './client'
import {
  Customer,
  FoodItem,
  Quote,
  Allergen,
  DietaryTag,
  AppSettings,
} from './types'

/**
 * Customer API services.
 */
export const customerService = {
  list: () => client.get<Customer[]>('/api/v1/customers'),
  get: (id: number) => client.get<Customer>(`/api/v1/customers/${id}`),
  create: (data: Omit<Customer, 'id' | 'createdAt'>) =>
    client.post<Customer>('/api/v1/customers', data),
  update: (id: number, data: Partial<Customer>) =>
    client.put<Customer>(`/api/v1/customers/${id}`, data),
  delete: (id: number) => client.delete(`/api/v1/customers/${id}`),
}

/**
 * Quote API services.
 */
export const quoteService = {
  list: () => client.get<Quote[]>('/api/v1/quotes'),
  get: (id: number) => client.get<Quote>(`/api/v1/quotes/${id}`),
  create: (data: Omit<Quote, 'id' | 'createdAt'>) =>
    client.post<Quote>('/api/v1/quotes', data),
  update: (id: number, data: Partial<Quote>) =>
    client.put<Quote>(`/api/v1/quotes/${id}`, data),
  delete: (id: number) => client.delete(`/api/v1/quotes/${id}`),
  send: (id: number, email: string) =>
    client.post(`/api/v1/quotes/${id}/send`, { email }),
}

/**
 * Food Item API services.
 */
export const foodItemService = {
  list: () => client.get<FoodItem[]>('/api/v1/food-items'),
  get: (id: number) => client.get<FoodItem>(`/api/v1/food-items/${id}`),
  create: (data: Omit<FoodItem, 'id' | 'createdAt'>) =>
    client.post<FoodItem>('/api/v1/food-items', data),
  update: (id: number, data: Partial<FoodItem>) =>
    client.put<FoodItem>(`/api/v1/food-items/${id}`, data),
  delete: (id: number) => client.delete(`/api/v1/food-items/${id}`),
}

/**
 * Reference data API services.
 */
export const referenceDataService = {
  allergens: () => client.get<Allergen[]>('/api/v1/allergens'),
  dietaryTags: () => client.get<DietaryTag[]>('/api/v1/dietary-tags'),
}

/**
 * Settings API services.
 */
export const settingsService = {
  get: () => client.get<AppSettings>('/api/v1/settings'),
  update: (data: Partial<AppSettings>) =>
    client.put<AppSettings>('/api/v1/settings', data),
}

/**
 * Health check services.
 */
export const healthService = {
  check: () => client.get('/health'),
}
