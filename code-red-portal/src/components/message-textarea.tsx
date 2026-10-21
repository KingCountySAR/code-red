"use client"

import { useState } from "react"

const WARN_LENGTH = 110;

export default function MessageTextarea({name}: {name: string}) {
  const [ length, setLength ] = useState<number>(0);
  const tooLong = length >= WARN_LENGTH;

  return (
    <>
      <textarea
        name={name}
        className={`flex textarea textarea-bordered ${tooLong ? 'textarea-warning' : ''}`}
        placeholder="Message Content"
        required
        onChange={evt => setLength(evt.currentTarget.value.length)}
        />
      <div className={`flex justify-end items-center gap-1 text-sm pt-1 ${tooLong ? 'text-warning' : 'opacity-60'}`}>
        {tooLong && (
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-4 h-4" aria-hidden="true">
            <path fillRule="evenodd" d="M8.485 2.495c.673-1.167 2.357-1.167 3.03 0l6.28 10.875c.673 1.167-.17 2.625-1.516 2.625H3.72c-1.347 0-2.189-1.458-1.515-2.625L8.485 2.495zM10 5a.75.75 0 01.75.75v3.5a.75.75 0 01-1.5 0v-3.5A.75.75 0 0110 5zm0 9a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
          </svg>
        )}
        <span title={tooLong ? "Messages over the limit will require the reader to click a link to get the full message" : undefined}>
          {length} / {WARN_LENGTH}
        </span>
      </div>
    </>
  )
}
