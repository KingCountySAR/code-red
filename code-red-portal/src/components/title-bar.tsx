import SignOut from "./sign-out";

export default function TitleBar(props: { team: string, name: string | undefined | null }) {
  const { team, name } = props;

  return (
    <div className="navbar bg-neutral text-neutral-content">
      <div className="navbar-start">
        <a className="text-xl">Page {team}</a>
      </div>
      <div className="navbar-end md:hidden">
      <div className="dropdown dropdown-end">
          <div tabIndex={0} role="button" className="btn btn-ghost md:hidden">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-5 w-5"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M4 6h16M4 12h8m-8 6h16" />
            </svg>
          </div>
          <ul
            tabIndex={0}
            className="menu menu-sm dropdown-content bg-base-100 text-base-content rounded-box z-[1] mt-3 w-52 p-2 shadow">
            <li className="p-3">{name}</li>
            <li><SignOut /></li>
          </ul>
        </div>
        </div>
      <div className="navbar-end hidden md:flex">
        <ul className="menu menu-horizontal items-center px-1">
          <li className="px-3">{name}</li>
          <li><SignOut /></li>
        </ul>
      </div>
    </div>
  )
}