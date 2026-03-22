# Cash Register — Frontend

React + TypeScript frontend for the Cash Register change calculator. Upload a transaction file, select a country and currency, and download the calculated change denominations as a text file.

---

## Dependencies

### Runtime
| Package | Version | Purpose |
|---|---|---|
| `react` | ^19.2.4 | UI framework |
| `react-dom` | ^19.2.4 | DOM rendering |
| `@mui/material` | ^7.3.9 | Component library |
| `@mui/icons-material` | ^7.3.9 | MUI icon set |
| `@emotion/react` | ^11.14.0 | MUI styling engine |
| `@emotion/styled` | ^11.14.1 | MUI styling engine |
| `axios` | ^1.13.6 | HTTP client |
| `react-dropzone` | ^15.0.0 | File drag-and-drop |
| `react-hot-toast` | ^2.6.0 | Toast notifications |

### Dev
| Package | Purpose |
|---|---|
| `vite` + `@vitejs/plugin-react` | Build tool and dev server |
| `typescript` | Type checking |
| `vitest` | Test runner |
| `@testing-library/react` | Component testing utilities |
| `@testing-library/user-event` | User interaction simulation |
| `@testing-library/jest-dom` | DOM assertion matchers |
| `jsdom` | DOM environment for tests |
| `eslint` + `typescript-eslint` | Linting |
| `prettier` | Code formatting |

---

## Getting Started

### Prerequisites
- Node.js 18+
- The ASP.NET Core API running (see `api/` README)

### Install

```bash
cd web_app
npm install
```

### Environment

Create a `.env` file in `web_app/`:

```env
VITE_API_URL=http://localhost:5158
VITE_API_URL_HTTPS=https://localhost:7165
VITE_USE_HTTPS=true
```

Set `VITE_USE_HTTPS=false` to use HTTP instead of HTTPS.

### Run

```bash
npm run dev        # start dev server at http://localhost:3000
npm run build      # production build
npm run preview    # preview production build
npm run lint       # run ESLint
npm run test       # run tests once
npm run test:watch # run tests in watch mode
```

---

## Folder Structure

```
web_app/
├── src/
│   ├── App.tsx                  # Root component — theme provider, toast config
│   ├── main.tsx                 # Entry point
│   ├── components/              # Shared UI components
│   │   ├── Header.tsx
│   │   ├── UploadArea.tsx
│   │   ├── UploadInfo.tsx
│   │   └── index.ts
│   ├── pages/                   # Full-page views
│   │   ├── LandingPage.tsx
│   │   ├── LoadingScreen.tsx
│   │   ├── ErrorScreen.tsx
│   │   └── index.ts
│   ├── endpoints/               # API client modules
│   │   ├── apiClient.ts         # Axios instance + error helper
│   │   ├── country/
│   │   ├── currency/
│   │   ├── denomination/
│   │   ├── rule/
│   │   ├── fileupload/
│   │   └── index.ts
│   ├── theme/                   # MUI theme configuration
│   │   ├── tokens.ts            # Raw design tokens (colors, shape, typography)
│   │   ├── theme.ts             # createTheme() factory for light/dark mode
│   │   └── index.ts
│   ├── utils/
│   │   ├── downloadBlob.ts      # Triggers browser file download from a Blob
│   │   └── index.ts
│   └── test/
│       ├── fixtures.ts          # Shared mock data for tests
│       └── setup.ts             # Vitest global setup (@testing-library/jest-dom)
├── vitest.config.ts
├── vite.config.ts
├── tsconfig.json
├── tsconfig.app.json
├── tsconfig.test.json
└── package.json
```

The `@/` path alias maps to `src/`, so `import { Header } from '@/components'` resolves to `src/components/index.ts`.

---

## Components

### `Header`
**Location:** `src/components/Header.tsx`

Sticky app bar containing the brand, country/currency dropdowns, and the light/dark mode toggle.

| Prop | Type | Description |
|---|---|---|
| `countries` | `Country[]` | List of countries to populate the dropdown |
| `headerData` | `HeaderState \| undefined` | Currently selected country and currency |
| `setHeaderData` | `Dispatch` | Updates the selected country/currency |
| `onToggleMode` | `() => void` | Toggles light/dark theme |

Selecting a country automatically sets the first available currency. The currency dropdown is disabled until a country is selected.

---

### `UploadArea`
**Location:** `src/components/UploadArea.tsx`

Drag-and-drop file upload zone powered by `react-dropzone`. Accepts `.txt`, `.csv`, and `.tsv` files.

| Prop | Type | Description |
|---|---|---|
| `file` | `File \| undefined` | Currently selected file |
| `setFile` | `Dispatch` | Updates the selected file |
| `selectedCurrency` | `Currency \| undefined` | Disables the dropzone when undefined |
| `onFileUpload` | `(file: File) => void` | Called via `useEffect` when `file` changes |

The dropzone is disabled when no currency is selected. Once a file is set, `onFileUpload` is triggered automatically via a `useEffect` watching the `file` prop.

---

### `UploadInfo`
**Location:** `src/components/UploadInfo.tsx`

Read-only guidelines panel showing the expected file format, example transaction lines, and usage notes.

| Prop | Type | Description |
|---|---|---|
| `decimalSeparator` | `string` | Adjusts example lines to match the selected currency (e.g. `.` for USD, `,` for EUR) |

---

### `LandingPage`
**Location:** `src/pages/LandingPage.tsx`

Main page. Fetches countries on mount, manages file upload state, and orchestrates the full upload flow.

- Displays `LoadingScreen` while fetching countries
- Displays `ErrorScreen` (with retry) if the fetch fails
- On successful load, renders `Header` + `UploadInfo` + `UploadArea`
- Calls `uploadFile()` when a file is selected, then triggers a download via `downloadBlob()`
- Shows toast notifications for loading, success, and error states

| Prop | Type | Description |
|---|---|---|
| `onToggleMode` | `() => void` | Passed through to `Header` |

---

### `LoadingScreen`
**Location:** `src/pages/LoadingScreen.tsx`

Full-screen loading indicator shown while the initial country data is being fetched. Displays a `CircularProgress` spinner with the app name.

No props.

---

### `ErrorScreen`
**Location:** `src/pages/ErrorScreen.tsx`

Full-screen error display shown when the country fetch fails.

| Prop | Type | Description |
|---|---|---|
| `message` | `string` | Error message to display. Defaults to `"Something went wrong."` |
| `onRetry` | `() => void \| undefined` | When provided, renders a "Try Again" button |

---

## Theme

The theme system uses a token-based approach:

- **`src/theme/tokens.ts`** — raw values (color scales, border radius, font family). Never referenced directly in components.
- **`src/theme/theme.ts`** — maps tokens to MUI semantic roles (`primary`, `background`, `success`, etc.) for both light and dark modes.
- Components consume theme values via `useTheme()` and `sx` props — no hardcoded color values in component files.

---

## API Integration

All API calls go through the Axios instance in `src/endpoints/apiClient.ts`. The base URL is selected at runtime via the `VITE_USE_HTTPS` environment variable.

| Module | Endpoint |
|---|---|
| `getAllCountries()` | `GET /country` |
| `uploadFile(file, uploadInfo)` | `POST /fileupload` |

The `uploadFile` call sends a `multipart/form-data` request with the file and `uploadInfo.CountryId` / `uploadInfo.CurrencyId`. The response is a `Blob` (text file) which is immediately downloaded via `downloadBlob()`.

Error responses from the API are also returned as `Blob` (text), and `getAxiosError()` reads the blob text to extract the message.
