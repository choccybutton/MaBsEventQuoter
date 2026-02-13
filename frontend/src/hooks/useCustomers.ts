import { useState, useCallback } from 'react'
import { Customer, ApiError } from '../api/types'
import { customerService } from '../api/services'
import { usePagination } from './usePagination'

export interface UseCustomersState {
  customers: Customer[]
  loading: boolean
  error: ApiError | null
}

export interface UseCustomersActions {
  fetchCustomers: (page?: number) => Promise<void>
  getCustomer: (id: number) => Promise<Customer | null>
  createCustomer: (data: Omit<Customer, 'id' | 'createdAt' | 'modifiedAt'>) => Promise<Customer | null>
  updateCustomer: (id: number, data: Partial<Customer>) => Promise<Customer | null>
  deleteCustomer: (id: number) => Promise<boolean>
  reset: () => void
}

export function useCustomers() {
  const [state, setState] = useState<UseCustomersState>({
    customers: [],
    loading: false,
    error: null,
  })

  const pagination = usePagination(20)

  const fetchCustomers = useCallback(
    async (page = 1) => {
      setState((prev) => ({ ...prev, loading: true, error: null }))
      try {
        const response = await customerService.list(page, pagination.pageSize)
        setState((prev) => ({
          ...prev,
          customers: response.data.items,
          loading: false,
        }))
        pagination.updatePagination({
          page: response.data.page,
          pageSize: response.data.pageSize,
          total: response.data.total,
          totalPages: response.data.totalPages,
          hasNextPage: response.data.hasNextPage,
          hasPreviousPage: response.data.hasPreviousPage,
        })
      } catch (error) {
        const apiError: ApiError = {
          message: error instanceof Error ? error.message : 'Failed to fetch customers',
        }
        setState((prev) => ({ ...prev, error: apiError, loading: false }))
      }
    },
    [pagination.pageSize, pagination]
  )

  const getCustomer = useCallback(async (id: number): Promise<Customer | null> => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      const response = await customerService.get(id)
      setState((prev) => ({ ...prev, loading: false }))
      return response.data
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to fetch customer',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return null
    }
  }, [])

  const createCustomer = useCallback(
    async (data: Omit<Customer, 'id' | 'createdAt' | 'modifiedAt'>) => {
      setState((prev) => ({ ...prev, loading: true, error: null }))
      try {
        const response = await customerService.create(data)
        const newCustomer = response.data
        setState((prev) => ({
          ...prev,
          customers: [newCustomer, ...prev.customers],
          loading: false,
        }))
        return newCustomer
      } catch (error) {
        const apiError: ApiError = {
          message: error instanceof Error ? error.message : 'Failed to create customer',
        }
        setState((prev) => ({ ...prev, error: apiError, loading: false }))
        return null
      }
    },
    []
  )

  const updateCustomer = useCallback(async (id: number, data: Partial<Customer>) => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      const response = await customerService.update(id, data)
      const updatedCustomer = response.data
      setState((prev) => ({
        ...prev,
        customers: prev.customers.map((c) => (c.id === id ? updatedCustomer : c)),
        loading: false,
      }))
      return updatedCustomer
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to update customer',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return null
    }
  }, [])

  const deleteCustomer = useCallback(async (id: number) => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      await customerService.delete(id)
      setState((prev) => ({
        ...prev,
        customers: prev.customers.filter((c) => c.id !== id),
        loading: false,
      }))
      return true
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to delete customer',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return false
    }
  }, [])

  const reset = useCallback(() => {
    setState({ customers: [], loading: false, error: null })
    pagination.reset()
  }, [pagination])

  return {
    ...state,
    pagination,
    fetchCustomers,
    getCustomer,
    createCustomer,
    updateCustomer,
    deleteCustomer,
    reset,
  }
}
