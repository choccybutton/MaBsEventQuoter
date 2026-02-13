import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useFoodItems } from '../hooks'
import '../styles/FoodItemsList.css'

export function FoodItemsList() {
  const { foodItems, loading, error, pagination, fetchFoodItems, deleteFoodItem } = useFoodItems()
  const [showInactive, setShowInactive] = useState(false)

  useEffect(() => {
    fetchFoodItems(pagination.page, !showInactive)
  }, [pagination.page, showInactive])

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this food item?')) {
      await deleteFoodItem(id)
    }
  }

  return (
    <div className="food-items-list">
      <header className="food-items-header">
        <h1>Food Items</h1>
        <Link to="/food-items/new" className="btn primary">
          + New Item
        </Link>
      </header>

      {error && <div className="error-message">{error.message}</div>}

      <div className="filters">
        <label className="checkbox-label">
          <input type="checkbox" checked={showInactive} onChange={(e) => setShowInactive(e.target.checked)} />
          Show inactive items
        </label>
      </div>

      {loading ? (
        <p>Loading food items...</p>
      ) : foodItems.length === 0 ? (
        <p>No food items found.</p>
      ) : (
        <>
          <table className="food-items-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Description</th>
                <th>Cost Price</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {foodItems.map((item) => (
                <tr key={item.id}>
                  <td>{item.name}</td>
                  <td>{item.description || '-'}</td>
                  <td>£{item.costPrice.toFixed(2)}</td>
                  <td>
                    <span className={`status-badge ${item.isActive ? 'active' : 'inactive'}`}>
                      {item.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="actions">
                    <Link to={`/food-items/${item.id}`} className="link-btn">
                      View
                    </Link>
                    <Link to={`/food-items/${item.id}/edit`} className="link-btn">
                      Edit
                    </Link>
                    <button onClick={() => handleDelete(item.id)} className="link-btn delete">
                      Delete
                    </button>
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
