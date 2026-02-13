import { useEffect, useState } from 'react'
import { ApiError } from '../api/types'
import { settingsService } from '../api/services'
import { useValidation } from '../hooks'
import '../styles/Settings.css'

export function Settings() {
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<ApiError | null>(null)
  const [success, setSuccess] = useState(false)

  const { errors, validatePercentage, clearAllErrors } = useValidation()

  const [defaultVatRate, setDefaultVatRate] = useState<number>(0.2)
  const [defaultMarkupPercentage, setDefaultMarkupPercentage] = useState<number>(0.5)
  const [marginGreenThresholdPct, setMarginGreenThresholdPct] = useState<number>(0.4)
  const [marginAmberThresholdPct, setMarginAmberThresholdPct] = useState<number>(0.2)

  useEffect(() => {
    loadSettings()
  }, [])

  const loadSettings = async () => {
    setLoading(true)
    setError(null)

    try {
      const response = await settingsService.get()
      const data = response.data

      setDefaultVatRate(data.defaultVatRate)
      setDefaultMarkupPercentage(data.defaultMarkupPercentage)
      setMarginGreenThresholdPct(data.marginGreenThresholdPct)
      setMarginAmberThresholdPct(data.marginAmberThresholdPct)
    } catch (err) {
      const apiError: ApiError = {
        message: err instanceof Error ? err.message : 'Failed to load settings',
      }
      setError(apiError)
    } finally {
      setLoading(false)
    }
  }

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault()

    clearAllErrors()

    // Validate all percentage fields
    const isVatValid = validatePercentage(defaultVatRate, 'defaultVatRate')
    const isMarkupValid = validatePercentage(defaultMarkupPercentage, 'defaultMarkupPercentage')
    const isGreenValid = validatePercentage(marginGreenThresholdPct, 'marginGreenThresholdPct')
    const isAmberValid = validatePercentage(marginAmberThresholdPct, 'marginAmberThresholdPct')

    if (!isVatValid || !isMarkupValid || !isGreenValid || !isAmberValid) {
      return
    }

    // Validate green > amber
    if (marginGreenThresholdPct <= marginAmberThresholdPct) {
      setError({
        message: 'Green threshold must be greater than Amber threshold',
      })
      return
    }

    setSaving(true)
    setError(null)
    setSuccess(false)

    try {
      await settingsService.update({
        defaultVatRate,
        defaultMarkupPercentage,
        marginGreenThresholdPct,
        marginAmberThresholdPct,
      })

      setSuccess(true)
      setTimeout(() => setSuccess(false), 3000)
    } catch (err) {
      const apiError: ApiError = {
        message: err instanceof Error ? err.message : 'Failed to save settings',
      }
      setError(apiError)
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <p>Loading settings...</p>

  return (
    <div className="settings">
      <header className="settings-header">
        <h1>Settings</h1>
        <p>Manage application-wide pricing and margin settings</p>
      </header>

      {error && (
        <div className="alert alert-error">
          <strong>Error:</strong> {error.message}
        </div>
      )}

      {success && (
        <div className="alert alert-success">
          <strong>Success!</strong> Settings have been saved.
        </div>
      )}

      <form onSubmit={handleSave} className="settings-form">
        {/* Default Pricing Section */}
        <section className="settings-section">
          <h2>Default Pricing</h2>
          <p className="section-description">
            These values will be used as defaults when creating new quotes.
          </p>

          <div className="form-group">
            <label htmlFor="defaultVatRate">
              Default VAT Rate (%) *
              <span className="help">0-100</span>
            </label>
            <div className="input-wrapper">
              <input
                id="defaultVatRate"
                type="number"
                min="0"
                max="100"
                step="0.1"
                value={defaultVatRate * 100}
                onChange={(e) => setDefaultVatRate(parseFloat(e.target.value) / 100)}
                className="form-control"
                required
              />
              <span className="percentage-sign">%</span>
            </div>
            {errors.defaultVatRate && (
              <span className="error">{errors.defaultVatRate[0]}</span>
            )}
            <div className="info">
              Current default: <strong>{(defaultVatRate * 100).toFixed(1)}%</strong>
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="defaultMarkup">
              Default Markup Percentage (%) *
              <span className="help">0-100</span>
            </label>
            <div className="input-wrapper">
              <input
                id="defaultMarkup"
                type="number"
                min="0"
                max="100"
                step="0.1"
                value={defaultMarkupPercentage * 100}
                onChange={(e) => setDefaultMarkupPercentage(parseFloat(e.target.value) / 100)}
                className="form-control"
                required
              />
              <span className="percentage-sign">%</span>
            </div>
            {errors.defaultMarkupPercentage && (
              <span className="error">{errors.defaultMarkupPercentage[0]}</span>
            )}
            <div className="info">
              Current default: <strong>{(defaultMarkupPercentage * 100).toFixed(1)}%</strong>
            </div>
          </div>
        </section>

        {/* Margin Thresholds Section */}
        <section className="settings-section">
          <h2>Margin Status Thresholds</h2>
          <p className="section-description">
            These thresholds determine the color coding of margins in quotes (Green, Amber, Red).
          </p>

          <div className="threshold-info">
            <div className="threshold-item">
              <span className="status-indicator green"></span>
              <strong>Green:</strong> Margin ≥ Green threshold
            </div>
            <div className="threshold-item">
              <span className="status-indicator amber"></span>
              <strong>Amber:</strong> Margin between Amber and Green threshold
            </div>
            <div className="threshold-item">
              <span className="status-indicator red"></span>
              <strong>Red:</strong> Margin &lt; Amber threshold
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="greenThreshold">
              Green Threshold (%) *
              <span className="help">0-100</span>
            </label>
            <div className="input-wrapper">
              <input
                id="greenThreshold"
                type="number"
                min="0"
                max="100"
                step="0.1"
                value={marginGreenThresholdPct * 100}
                onChange={(e) => setMarginGreenThresholdPct(parseFloat(e.target.value) / 100)}
                className="form-control"
                required
              />
              <span className="percentage-sign">%</span>
            </div>
            {errors.marginGreenThresholdPct && (
              <span className="error">{errors.marginGreenThresholdPct[0]}</span>
            )}
            <div className="info">
              Current: <strong>{(marginGreenThresholdPct * 100).toFixed(1)}%</strong>
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="amberThreshold">
              Amber Threshold (%) *
              <span className="help">0-100, must be less than Green</span>
            </label>
            <div className="input-wrapper">
              <input
                id="amberThreshold"
                type="number"
                min="0"
                max="100"
                step="0.1"
                value={marginAmberThresholdPct * 100}
                onChange={(e) => setMarginAmberThresholdPct(parseFloat(e.target.value) / 100)}
                className="form-control"
                required
              />
              <span className="percentage-sign">%</span>
            </div>
            {errors.marginAmberThresholdPct && (
              <span className="error">{errors.marginAmberThresholdPct[0]}</span>
            )}
            <div className="info">
              Current: <strong>{(marginAmberThresholdPct * 100).toFixed(1)}%</strong>
            </div>
          </div>

          <div className="threshold-visualization">
            <h3>Visual Guide</h3>
            <div className="margin-scale">
              <div className="scale-segment green" style={{ width: `${100 - marginGreenThresholdPct * 100}%` }}>
                <span>Green</span>
              </div>
              <div
                className="scale-segment amber"
                style={{
                  width: `${(marginGreenThresholdPct - marginAmberThresholdPct) * 100}%`,
                }}
              >
                <span>Amber</span>
              </div>
              <div className="scale-segment red" style={{ width: `${marginAmberThresholdPct * 100}%` }}>
                <span>Red</span>
              </div>
            </div>
            <div className="scale-labels">
              <span>0%</span>
              <span>{(marginAmberThresholdPct * 100).toFixed(1)}%</span>
              <span>{(marginGreenThresholdPct * 100).toFixed(1)}%</span>
              <span>100%</span>
            </div>
          </div>
        </section>

        {/* Actions */}
        <section className="settings-actions">
          <button type="submit" className="btn primary" disabled={saving}>
            {saving ? 'Saving...' : 'Save Settings'}
          </button>
          <button
            type="button"
            className="btn secondary"
            onClick={loadSettings}
            disabled={saving}
          >
            Reset to Saved
          </button>
        </section>
      </form>
    </div>
  )
}
