import { useState, useCallback } from 'react'

export interface PaginationState {
  page: number
  pageSize: number
  total: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export function usePagination(initialPageSize = 20) {
  const [pagination, setPagination] = useState<PaginationState>({
    page: 1,
    pageSize: initialPageSize,
    total: 0,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  })

  const updatePagination = useCallback((data: Partial<PaginationState>) => {
    setPagination((prev) => ({ ...prev, ...data }))
  }, [])

  const goToPage = useCallback((page: number) => {
    setPagination((prev) => ({ ...prev, page }))
  }, [])

  const goToNextPage = useCallback(() => {
    setPagination((prev) => ({
      ...prev,
      page: prev.hasNextPage ? prev.page + 1 : prev.page,
    }))
  }, [])

  const goToPreviousPage = useCallback(() => {
    setPagination((prev) => ({
      ...prev,
      page: prev.hasPreviousPage ? prev.page - 1 : prev.page,
    }))
  }, [])

  const reset = useCallback(() => {
    setPagination({
      page: 1,
      pageSize: initialPageSize,
      total: 0,
      totalPages: 0,
      hasNextPage: false,
      hasPreviousPage: false,
    })
  }, [initialPageSize])

  return {
    ...pagination,
    updatePagination,
    goToPage,
    goToNextPage,
    goToPreviousPage,
    reset,
  }
}
