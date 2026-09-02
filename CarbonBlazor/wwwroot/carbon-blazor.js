export function focusFirst(selector) {
  const root = document.querySelector(selector);
  const first = getFocusable(root)[0];
  if (first) first.focus();
}

export function focusById(id) {
  const element = document.getElementById(id);
  if (element) element.focus();
}

export function setBodyScrollLock(locked) {
  document.body.classList.toggle('cb-scroll-lock', locked);
}

export function clampTooltipToViewport(id, axis) {
  const el = document.getElementById(id);
  if (!el) return;
  const prop = axis === 'y' ? '--cb-tooltip-shift-y' : '--cb-tooltip-shift-x';
  el.style.setProperty(prop, '0px');
  const margin = 8;
  const rect = el.getBoundingClientRect();
  let shift = 0;
  if (axis === 'y') {
    if (rect.top < margin) shift = margin - rect.top;
    else if (rect.bottom > window.innerHeight - margin) shift = (window.innerHeight - margin) - rect.bottom;
  } else {
    if (rect.left < margin) shift = margin - rect.left;
    else if (rect.right > window.innerWidth - margin) shift = (window.innerWidth - margin) - rect.right;
  }
  if (shift) el.style.setProperty(prop, `${shift}px`);
}

export function trapFocus(rootId, event) {
  if (event.key !== 'Tab') return;
  const root = document.getElementById(rootId);
  const focusable = getFocusable(root);
  if (!focusable.length) return;

  const first = focusable[0];
  const last = focusable[focusable.length - 1];
  if (event.shiftKey && document.activeElement === first) {
    event.preventDefault();
    last.focus();
  } else if (!event.shiftKey && document.activeElement === last) {
    event.preventDefault();
    first.focus();
  }
}

export function clickOutside(rootId, dotNetRef, methodName) {
  const root = document.getElementById(rootId);
  if (!root) return;

  const previousController = outsideClickControllers.get(rootId);
  previousController?.abort();

  const controller = new AbortController();
  outsideClickControllers.set(rootId, controller);

  const cleanup = () => {
    if (outsideClickControllers.get(rootId) === controller) {
      outsideClickControllers.delete(rootId);
    }
  };

  const listen = () => {
    if (controller.signal.aborted) return;
    document.addEventListener('click', handler, { once: true, capture: true, signal: controller.signal });
  };

  const handler = (event) => {
    if (root.contains(event.target)) {
      listen();
      return;
    }

    cleanup();
    dotNetRef.invokeMethodAsync(methodName).catch(() => {});
  };

  controller.signal.addEventListener('abort', cleanup, { once: true });
  window.setTimeout(() => {
    if (outsideClickControllers.get(rootId) !== controller) return;
    listen();
  }, 0);
}

export function disposeClickOutside(rootId) {
  const controller = outsideClickControllers.get(rootId);
  controller?.abort();
}

export function rove(rootId, nextIndex) {
  const root = document.getElementById(rootId);
  if (!root) return;
  const items = [...root.querySelectorAll('[data-roving-item]')];
  if (!items.length) return;
  items.forEach((item, index) => item.tabIndex = index === nextIndex ? 0 : -1);
  items[nextIndex]?.focus();
}

export function moveTreeFocus(rootId, delta) {
  const root = document.getElementById(rootId);
  if (!root) return;

  const items = [...root.querySelectorAll('[data-roving-item]')]
    .filter((item) => item.offsetParent !== null && !item.disabled);
  if (!items.length) return;

  const currentIndex = Math.max(0, items.indexOf(document.activeElement));
  const nextIndex = Math.min(items.length - 1, Math.max(0, currentIndex + delta));
  items.forEach((item, index) => item.tabIndex = index === nextIndex ? 0 : -1);
  items[nextIndex]?.focus();
}

export async function copyElementText(id) {
  const el = document.getElementById(id);
  const text = el ? el.textContent ?? '' : '';
  try {
    await navigator.clipboard.writeText(text);
    return true;
  } catch {
    return false;
  }
}

export function matchesMedia(query) {
  return window.matchMedia(query).matches;
}

export function ensureStyles(href) {
  if (document.querySelector('link[data-carbon-blazor]')) return;
  const link = document.createElement('link');
  link.rel = 'stylesheet';
  link.href = href;
  link.setAttribute('data-carbon-blazor', '');
  document.head.appendChild(link);
}

function getFocusable(root) {
  if (!root) return [];
  return [...root.querySelectorAll('a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])')]
    .filter((element) => !element.hasAttribute('disabled') && !element.getAttribute('aria-hidden'));
}

const outsideClickControllers = new Map();
