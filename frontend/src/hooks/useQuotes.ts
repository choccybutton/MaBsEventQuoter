import { useState, useCallback } from 'react'
import { Quote, ApiError } from '../api/types'
import { quoteService } from '../api/services'
import { usePagination } from './usePagination'

export interface UseQuotesState {
  quotes: Quote[]
  loading: boolean
  error: ApiError | null
}

export interface UseQuotesActions {
  fetchQuotes: (page?: number, status?: string, customerId?: number) => Promise<void>
  getQuote: (id: number) => Promise<Quote | null>
  createQuote: (data: Omit<Quote, 'id' | 'createdAt' | 'modifiedAt' | 'sentAt'>) => Promise<Quote | null>
  updateQuote: (id: number, data: Partial<Quote>) => Promise<Quote | null>
  deleteQuote: (id: number) => Promise<boolean>
  sendQuote: (id: number, email: string) => Promise<boolean>
  reset: () => void
}

export function useQuotes() {
  const [state, setState] = useState<UseQuotesState>({
    quotes: [],
    loading: false,
    error: null,
  })

  const pagination = usePagination(20)

  const fetchQuotes = useCallback(
    async (page = 1, status?: string, customerId?: number) => {
      setState((prev) => ({ ...prev, loading: true, error: null }))
      try {
        const response = await quoteService.list(page, pagination.pageSize, status, customerId)
        setState((prev) => ({
          ...prev,
          quotes: response.data.items,
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
          message: error instanceof Error ? error.message : 'Failed to fetch quotes',
        }
        setState((prev) => ({ ...prev, error: apiError, loading: false }))
      }
    },
    [pagination.pageSize, pagination]
  )

  const getQuote = useCallback(async (id: number): Promise<Quote | null> => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      const response = await quoteService.get(id)
      setState((prev) => ({ ...prev, loading: false }))
      return response.data
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to fetch quote',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return null
    }
  }, [])

  const createQuote = useCallback(
    async (data: Omit<Quote, 'id' | 'createdAt' | 'modifiedAt' | 'sentAt'>) => {
      setState((prev) => ({ ...prev, loading: true, error: null }))
      try {
        const response = await quoteService.create(data)
        const newQuote = response.data
        setState((prev) => ({
          ...prev,
          quotes: [newQuote, ...prev.quotes],
          loading: false,
        }))
        return newQuote
      } catch (error) {
        const apiError: ApiError = {
          message: error instanceof Error ? error.message : 'Failed to create quote',
        }
        setState((prev) => ({ ...prev, error: apiError, loading: false }))
        return null
      }
    },
    []
  )

  const updateQuote = useCallback(async (id: number, data: Partial<Quote>) => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      const response = await quoteService.update(id, data)
      const updatedQuote = response.data
      setState((prev) => ({
        ...prev,
        quotes: prev.quotes.map((q) => (q.id === id ? updatedQuote : q)),
        loading: false,
      }))
      return updatedQuote
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to update quote',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return null
    }
  }, [])

  const deleteQuote = useCallback(async (id: number) => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      await quoteService.delete(id)
      setState((prev) => ({
        ...prev,
        quotes: prev.quotes.filter((q) => q.id !== id),
        loading: false,
      }))
      return true
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to delete quote',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return false
    }
  }, [])

  const sendQuote = useCallback(async (id: number, email: string) => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      await quoteService.send(id, email)
      setState((prev) => ({ ...prev, loading: false }))
      return true
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to send quote',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return false
    }
  }, [])

  const reset = useCallback(() => {
    setState({ quotes: [], loading: false, error: null })
    pagination.reset()
  }, [pagination])

  return {
    ...state,
    pagination,
    fetchQuotes,
    getQuote,
    createQuote,
    updateQuote,
    deleteQuote,
    sendQuote,
    reset,
  }
}
