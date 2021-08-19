import React from 'react';

class ScGlobalLogo extends React.Component {
    render() {
        return (
            <div className="gh-logo">
                <a className="global-logo" href="/sitecore/client/Applications/Launchpad">
                    <svg height="1024px" version="1.1" viewBox="0 0 1024 1024" width="1024px" xmlns="http://www.w3.org/2000/svg">
                        <defs>
                            <linearGradient id="lineargradient-1" x1="50%" x2="50%" y1="67.8691008%" y2="0%">
                                <stop offset="0%" stopColor="#BDBDBD"></stop>
                                <stop offset="100%" stopColor="#EBEBEB"></stop>
                            </linearGradient>
                        </defs>
                        <g fill="none" fillRule="evenodd" stroke="none" strokeWidth="1">
                            <g>
                                <path d="M0,0 L320,0 L320,320 L0,320 L0,0 Z M0,352 L320,352 L320,672 L0,672 L0,352 Z M0,704 L320,704 L320,1024 L0,1024 L0,704 Z M352,0 L672,0 L672,320 L352,320 L352,0 Z M352,352 L672,352 L672,672 L352,672 L352,352 Z M352,704 L672,704 L672,1024 L352,1024 L352,704 Z M704,0 L1024,0 L1024,320 L704,320 L704,0 Z M704,352 L1024,352 L1024,672 L704,672 L704,352 Z" fill="url(#lineargradient-1)"></path>
                                <rect fill="#DC291E" height="320" width="320" x="704" y="704"></rect>
                            </g>
                        </g>
                    </svg>
                    <svg height="1024px" version="1.1" viewBox="0 0 1024 1024" width="1024px" xmlns="http://www.w3.org/2000/svg">
                        <g fill="none" fillRule="evenodd" stroke="none" strokeWidth="1">
                            <g>
                                <path d="M0,704 L320,704 L320,1024 L0,1024 L0,704 Z M352,352 L672,352 L672,672 L352,672 L352,352 Z M352,704 L672,704 L672,1024 L352,1024 L352,704 Z M704,0 L1024,0 L1024,320 L704,320 L704,0 Z M704,352 L1024,352 L1024,672 L704,672 L704,352 Z" fill="#E54B43"></path>
                                <rect fill="#F4B2AE" height="320" width="320" x="0" y="0"></rect>
                                <rect fill="#ED827D" height="320" width="320" x="0" y="352"></rect>
                                <rect fill="#ED827D" height="320" width="320" x="352" y="0"></rect>
                                <rect fill="#DC291E" height="320" width="320" x="704" y="704"></rect>
                            </g>
                        </g>
                    </svg>
                </a>
            </div>
        );
    }
}

export default ScGlobalLogo;
