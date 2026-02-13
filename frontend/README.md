# Catering Quotes Frontend

React + TypeScript + Vite application for managing catering quotes.

## Quick Start

### Prerequisites
- Node.js LTS (18+)
- npm or yarn

### Development

```bash
# Install dependencies
npm install

# Start dev server (http://localhost:3000)
npm run dev

# Type checking
npm run typecheck

# Linting
npm run lint

# Format code
npm run format
```

### Building

```bash
# Build for production
npm run build

# Preview production build
npm run preview
```

### Testing

```bash
# Run tests
npm run test

# Generate coverage report
npm run test:coverage
```

## Project Structure

```
src/
├── main.tsx           # Entry point
├── App.tsx            # Main app component
├── App.css            # App styles
├── index.css          # Global styles
└── api/
    ├── client.ts      # Axios client with interceptors
    ├── types.ts       # TypeScript type definitions
    └── services.ts    # API service functions
```

## Environment Configuration

Environment variables are configured via `.env` files:

```bash
VITE_API_URL=http://localhost:5000
VITE_APP_ENV=development
```

Available in components via `import.meta.env.VITE_*`

## API Client

API client is pre-configured with:
- Base URL from `VITE_API_URL`
- Request/response interceptors
- Error handling
- 30-second timeout

Usage:
```typescript
import { quoteService, customerService } from './api/services'

// List quotes
const { data } = await quoteService.list()

// Get specific customer
const { data } = await customerService.get(1)

// Create quote
const { data } = await quoteService.create(quoteData)
```

## Development Guidelines

### Code Style
- ESLint for linting
- Prettier for formatting
- TypeScript strict mode enabled
- React hooks best practices

### Scripts
- `npm run dev` - Start development server
- `npm run build` - Production build
- `npm run lint` - Run ESLint
- `npm run format` - Format code with Prettier
- `npm run format:check` - Check if formatting needed
- `npm run typecheck` - Type check without build
- `npm run test` - Run tests

## Deployment

### Production Build
```bash
npm run build
```

Generates optimized bundle in `dist/` directory.

### Docker
Dockerfile available at `../deploy/Dockerfile.frontend` (create during deployment phase).

## Features (MVP)

- ✅ React 18 with TypeScript
- ✅ Vite for fast development
- ✅ API client with Axios
- ✅ TypeScript type safety
- ✅ ESLint + Prettier
- ✅ Vitest for unit testing
- ✅ Environment configuration
- ✅ Basic styling

## Future Features (Phase 2+)

- State management (Context API or Redux)
- Routing (React Router)
- Form validation
- Advanced UI components
- E2E testing with Cypress/Playwright
- Performance monitoring
- Analytics integration

## Troubleshooting

### API Connection Issues
- Ensure API is running on `VITE_API_URL`
- Check browser console for CORS errors
- Verify environment variables in `.env`

### Build Issues
- Clear `node_modules` and reinstall: `npm ci`
- Clear Vite cache: `npm run build` with `--force` flag

### Port Already in Use
- Dev server uses port 3000 by default
- Change in `vite.config.ts` if needed

## Resources

- [React Documentation](https://react.dev)
- [TypeScript Handbook](https://www.typescriptlang.org/docs)
- [Vite Guide](https://vitejs.dev/guide)
- [Axios Documentation](https://axios-http.com)
