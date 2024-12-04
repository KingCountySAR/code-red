"use client"

import { useState } from "react"

export default function OneShotSubmit({text}: {text: string}) {
  const [ clicked, setClicked ] = useState<boolean>(false);

  return (
    <button
      className="btn btn-primary"
      type="submit"
      onClick={evt => { setClicked(true); evt.currentTarget.closest('form')?.requestSubmit(); }}
      disabled={clicked}
      >{text}</button>
  )
}