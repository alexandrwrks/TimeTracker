from pathlib import Path

from fastapi import FastAPI, HTTPException, status
from fastapi.responses import FileResponse

app = FastAPI(title="TimeTracker API")

PROJECT_DIR = Path(__file__).resolve().parent.parent

APPLICATION_FILE = (
        PROJECT_DIR
        / "downloads"
        / "TimeTracker.exe"
)


@app.get("/")
async def root():
    return {"message": "Welcome to TimeTracker API!"}


@app.get("/download/timetracker")
async def application():
    if not APPLICATION_FILE.is_file():
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Файл TimeTracker не был найден на сервере"
        )

    return FileResponse(
        path=APPLICATION_FILE,
        filename="TimeTracker-0.1.0-win-x64.exe",
        media_type="application/octet-stream",
    )