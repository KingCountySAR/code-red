"use client"

import Link from "next/link"
import { useSearchParams } from "next/navigation"

export default function SuccessPage() {
  const search = useSearchParams()
  const message = search.get("message");

  return (
    <div className="flex h-screen w-full flex-col items-center pt-12">
      <div
        className="block max-w-sm rounded-lg border border-gray-200 bg-white p-6 text-center shadow dark:border-gray-700 dark:bg-gray-800"
      >
        <div role="alert" className="alert alert-success">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-6 w-6 shrink-0 stroke-current"
            fill="none"
            viewBox="0 0 24 24">
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span>Message sent!</span>
        </div>
        <div className="my-5 font-normal text-gray-700 dark:text-gray-400">
          {message}
        </div>
        <div>
          <Link className="btn btn-sm" href="/">Send New Message</Link>
        </div>
      </div>
    </div>
  )
}