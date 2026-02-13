import { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useCustomers } from '../hooks'
import '../styles/CustomersList.css'

export function CustomersList() {
  const { customers, loading, error, pagination, fetchCustomers, deleteCustomer } = useCustomers()

  useEffect(() => {
    fetchCustomers(pagination.page)
  }, [pagination.page])

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this customer?')) {
      await deleteCustomer(id)
    }
  }

  return (
    <div className="customers-list">
      <header className="customers-header">
        <h1>Customers</h1>
        <Link to="/customers/new" className="btn primary">
          + New Customer
        </Link>
      </header>

      {error && <div className="error-message">{error.message}</div>}

      {loading ? (
        <p>Loading customers...</p>
      ) : customers.length === 0 ? (
        <p>No customers found.</p>
      ) : (
        <>
          <table className="customers-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Phone</th>
                <th>Company</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {customers.map((customer) => (
                <tr key={customer.id}>
                  <td>{customer.name}</td>
                  <td>{customer.email}</td>
                  <td>{customer.phone || '-'}</td>
                  <td>{customer.company || '-'}</td>
                  <td className="actions">
                    <Link to={`/customers/${customer.id}`} className="link-btn">
                      View
                    </Link>
                    <Link to={`/customers/${customer.id}/edit`} className="link-btn">
                      Edit
                    </Link>
                    <button onClick={() => handleDelete(customer.id)} className="link-btn delete">
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
