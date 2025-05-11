#!/usr/bin/env bash
set -euo pipefail

pkill -f dotnet

# -------- 設定 --------
PROJECT_ROOT="$HOME/Projects/BookVisionWeb"
WEB_CSPROJ="$PROJECT_ROOT/BookVisionWeb.Web/BookVisionWeb.Web.csproj"
IMG_PATH="$PROJECT_ROOT/sample.jpg"          # アップロードする画像
PORT=5040
WAIT_MAX=15                                  # ポート待機タイムアウト秒

# --- macOS Apple‑Silicon dylib fallback ---------------------------------
if [[ "$(uname)" == "Darwin" ]]; then
  # Resolve the actual leptonica dylib installed by Homebrew (e.g. libleptonica.7.dylib)
  SRC=$(ls /opt/homebrew/lib/libleptonica.*.dylib 2>/dev/null | head -n 1)
  if [[ -n "$SRC" ]]; then
    for V in 1.80.0 1.82.0; do
      for DIR in /usr/local/lib /opt/homebrew/lib; do
        TARGET="$DIR/libleptonica-${V}.dylib"
        if [[ ! -e "$TARGET" ]]; then
          echo "🔧  Linking $TARGET → $(basename "$SRC")"
          sudo mkdir -p "$DIR"
          sudo ln -sf "$SRC" "$TARGET"
        fi
      done
    done
  fi

  # Ensure libtesseract is also visible
  for DLL in /opt/homebrew/lib/libtesseract.*.dylib; do
    [[ -e "$DLL" ]] || continue
    BASENAME=$(basename "$DLL")
    for DIR in /usr/local/lib /opt/homebrew/lib; do
      TARGET="$DIR/$BASENAME"
      if [[ ! -e "$TARGET" ]]; then
        sudo mkdir -p "$DIR"
        sudo ln -sf "$DLL" "$TARGET"
      fi
    done
  done

  # Library search path for current shell
  export DYLD_LIBRARY_PATH="/usr/local/lib:/opt/homebrew/lib:${DYLD_LIBRARY_PATH:-}"
  export DYLD_FALLBACK_LIBRARY_PATH="/usr/local/lib:/opt/homebrew/lib:${DYLD_FALLBACK_LIBRARY_PATH:-}"
fi
# ------------------------------------------------------------------------


echo "▶️  Building & starting Web API..."
dotnet run --project "$WEB_CSPROJ" &    # バックグラウンド起動
APP_PID=$!

# ポートが LISTEN するまで待機
for ((i=0; i<WAIT_MAX; i++)); do
  if lsof -i :"$PORT" >/dev/null 2>&1; then
    echo "✅  API is up (port $PORT)"
    break
  fi
  sleep 1
done

if ! lsof -i :"$PORT" >/dev/null 2>&1; then
  echo "❌  API did not start within ${WAIT_MAX}s"
  kill "$APP_PID"
  exit 1
fi

echo "📤  Uploading image: $IMG_PATH"
PAGE_ID=$(curl -s -F "file=@$IMG_PATH" "http://localhost:$PORT/api/pages" | jq -r .pageId)
echo "🆔  Received pageId: $PAGE_ID"

echo "🔎  Running OCR..."
OCR_JSON=$(curl -s -X POST "http://localhost:$PORT/api/pages/$PAGE_ID/ocr")
echo "📝  OCR result:"
echo "$OCR_JSON" | jq .

echo "🛑  Stopping API (pid $APP_PID)"
kill "$APP_PID"
