import { useState, useCallback } from 'react'
import { FoodItem, ApiError } from '../api/types'
import { foodItemService } from '../api/services'
import { usePagination } from './usePagination'

export interface UseFoodItemsState {
  foodItems: FoodItem[]
  loading: boolean
  error: ApiError | null
}

export interface UseFoodItemsActions {
  fetchFoodItems: (page?: number, activeOnly?: boolean) => Promise<void>
  getFoodItem: (id: number) => Promise<FoodItem | null>
  createFoodItem: (data: Omit<FoodItem, 'id' | 'createdAt' | 'modifiedAt'>) => Promise<FoodItem | null>
  updateFoodItem: (id: number, data: Partial<FoodItem>) => Promise<FoodItem | null>
  deleteFoodItem: (id: number) => Promise<boolean>
  reset: () => void
}

export function useFoodItems() {
  const [state, setState] = useState<UseFoodItemsState>({
    foodItems: [],
    loading: false,
    error: null,
  })

  const pagination = usePagination(20)

  const fetchFoodItems = useCallback(
    async (page = 1, activeOnly = false) => {
      setState((prev) => ({ ...prev, loading: true, error: null }))
      try {
        const response = await foodItemService.list(page, pagination.pageSize, activeOnly)
        setState((prev) => ({
          ...prev,
          foodItems: response.data.items,
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
          message: error instanceof Error ? error.message : 'Failed to fetch food items',
        }
        setState((prev) => ({ ...prev, error: apiError, loading: false }))
      }
    },
    [pagination.pageSize, pagination]
  )

  const getFoodItem = useCallback(async (id: number): Promise<FoodItem | null> => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      const response = await foodItemService.get(id)
      setState((prev) => ({ ...prev, loading: false }))
      return response.data
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to fetch food item',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return null
    }
  }, [])

  const createFoodItem = useCallback(
    async (data: Omit<FoodItem, 'id' | 'createdAt' | 'modifiedAt'>) => {
      setState((prev) => ({ ...prev, loading: true, error: null }))
      try {
        const response = await foodItemService.create(data)
        const newFoodItem = response.data
        setState((prev) => ({
          ...prev,
          foodItems: [newFoodItem, ...prev.foodItems],
          loading: false,
        }))
        return newFoodItem
      } catch (error) {
        const apiError: ApiError = {
          message: error instanceof Error ? error.message : 'Failed to create food item',
        }
        setState((prev) => ({ ...prev, error: apiError, loading: false }))
        return null
      }
    },
    []
  )

  const updateFoodItem = useCallback(async (id: number, data: Partial<FoodItem>) => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      const response = await foodItemService.update(id, data)
      const updatedFoodItem = response.data
      setState((prev) => ({
        ...prev,
        foodItems: prev.foodItems.map((f) => (f.id === id ? updatedFoodItem : f)),
        loading: false,
      }))
      return updatedFoodItem
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to update food item',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return null
    }
  }, [])

  const deleteFoodItem = useCallback(async (id: number) => {
    setState((prev) => ({ ...prev, loading: true, error: null }))
    try {
      await foodItemService.delete(id)
      setState((prev) => ({
        ...prev,
        foodItems: prev.foodItems.filter((f) => f.id !== id),
        loading: false,
      }))
      return true
    } catch (error) {
      const apiError: ApiError = {
        message: error instanceof Error ? error.message : 'Failed to delete food item',
      }
      setState((prev) => ({ ...prev, error: apiError, loading: false }))
      return false
    }
  }, [])

  const reset = useCallback(() => {
    setState({ foodItems: [], loading: false, error: null })
    pagination.reset()
  }, [pagination])

  return {
    ...state,
    pagination,
    fetchFoodItems,
    getFoodItem,
    createFoodItem,
    updateFoodItem,
    deleteFoodItem,
    reset,
  }
}
